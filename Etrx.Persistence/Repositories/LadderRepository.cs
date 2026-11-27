using System.Numerics;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Repositories;
using Etrx.Domain.Expressions;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Queries;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public sealed class LadderRepository(EtrxDbContext db) : ILadderRepository
{
    public async Task<List<LadderLevel>> GetLevelsAsync(CancellationToken cancellationToken)
    {
        var difficulty = ProblemExpressions.DifficultyExpression;
        return await db.Problems.AsExpandable().Where(p => p.Contest.Division != "")
            .Select(p => new { p.Contest.Division, Difficulty = difficulty.Invoke(p) })
            .Distinct().OrderBy(p => p.Division).ThenBy(p => p.Difficulty)
            .Select(p => new LadderLevel(p.Division, p.Difficulty)).ToListAsync(cancellationToken);
    }

    public Task<List<int>> GetRatingsAsync(LadderQuery query, CancellationToken cancellationToken) =>
        GetLevel(new LadderQuery { Division = query.Division })
            .Select(p => p.Rating).Distinct().OrderBy(rating => rating).ToListAsync(cancellationToken);

    public Task<List<int>> GetDifficultiesAsync(LadderQuery query, CancellationToken cancellationToken)
    {
        var difficulty = ProblemExpressions.DifficultyExpression;
        return GetLevel(new LadderQuery { Division = query.Division, Rating = query.Rating })
            .Select(p => difficulty.Invoke(p)).Distinct().OrderBy(value => value).ToListAsync(cancellationToken);
    }

    private IQueryable<Problem> GetLevel(LadderQuery query)
    {
        var difficulty = ProblemExpressions.DifficultyExpression;
        var problems = db.Problems.AsNoTracking().AsExpandable()
            .Where(p => p.Contest.Division == query.Division);
        if (query.Difficulty.HasValue)
            problems = problems.Where(p => difficulty.Invoke(p) == query.Difficulty);
        if (query.Rating.HasValue)
            problems = problems.Where(p => p.Rating == query.Rating);
        return problems;
    }

    private static IQueryable<Problem> SelectTags(IQueryable<Problem> problems, string[] tags) =>
        tags.Length == 0 ? problems : problems.Where(p =>
            p.Tags.Any() && p.Tags.All(t => tags.Contains(t.Name)));

    public async Task<LadderResponse> GetLadderAsync(LadderQuery query, CancellationToken cancellationToken, Guid? userId = null)
    {
        var level = GetLevel(query);
        var available = await level.SelectMany(p => p.Tags).GroupBy(t => t.Name)
            .Select(g => new { Name = g.Key, Priority = g.Max(t => t.Priority) })
            .OrderByDescending(t => t.Priority).ThenBy(t => t.Name)
            .Select(t => new LadderTag(t.Name, t.Priority)).ToArrayAsync(cancellationToken);

        var rows = await SelectTags(level, query.Tags)
            .OrderByDescending(p => p.SolvedCount).ThenBy(p => p.ContestId).ThenBy(p => p.Index)
            .Select(p => new
            {
                p.Id,
                p.ContestId,
                p.Index,
                Tags = p.Tags.Select(t => new LadderTag(t.Name, t.Priority)).ToList()
            }).ToListAsync(cancellationToken);

        // Each harder tag introduces the next block: A, B, A+B, C, A+C, B+C, A+B+C.
        // Rank only combinations present in the catalogue, regardless of how many tags exist.
        var ranked = rows.SelectMany(r => r.Tags).DistinctBy(t => t.Name)
            .OrderByDescending(t => t.Priority).ThenBy(t => t.Name, StringComparer.Ordinal).ToArray();
        var bits = ranked.Select((t, i) => (t.Name, Bit: BigInteger.One << i)).ToDictionary(t => t.Name, t => t.Bit);
        var groups = rows.GroupBy(r => r.Tags.Aggregate(BigInteger.Zero, (mask, t) => mask | bits[t.Name]))
            .OrderBy(g => g.Key).ToArray();
        var page = groups.Skip(query.GroupOffset).Take(query.GroupLimit ?? int.MaxValue).ToArray();
        var solved = new HashSet<(int, string)>();
        var statuses = new Dictionary<(int, string), string>();
        if (userId.HasValue && page.Length > 0)
        {
            // Use the same saved progress and imported submissions as the Problems page.
            // Branch completion includes every task, even while the branch is collapsed.
            var contestIds = page.SelectMany(g => g).Select(r => r.ContestId).Distinct().ToArray();
            var progress = await ProblemProgressQuery.ForUser(db, userId.Value)
                .Where(p => contestIds.Contains(p.ContestId))
                .Select(p => new { p.ContestId, p.Index, p.Status }).ToListAsync(cancellationToken);
            statuses = progress.ToDictionary(p => (p.ContestId, p.Index), p => p.Status);
            solved = progress.Where(p => p.Status == ProblemStatuses.Solved)
                .Select(p => (p.ContestId, p.Index)).ToHashSet();
        }
        var tasks = new Dictionary<(int, string), LadderProblem>();
        if (query.IncludeProblems && page.Length > 0)
        {
            var ids = page.SelectMany(g => g).Select(r => r.Id).ToArray();
            var selected = await db.Problems.AsNoTracking().Where(p => ids.Contains(p.Id))
                .Select(p => new LadderProblem(p.ContestId, p.Index,
                    p.ProblemTranslations.OrderByDescending(t => t.LanguageCode == query.Lang)
                        .ThenBy(t => t.LanguageCode).Select(t => t.Name).FirstOrDefault() ?? p.Index,
                    p.Rating, p.SolvedCount, p.Contest.Gym, p.Contest.Source, false, ProblemStatuses.None))
                .ToArrayAsync(cancellationToken);
            tasks = selected.ToDictionary(p => (p.ContestId, p.Index),
                p => p with
                {
                    IsSolved = solved.Contains((p.ContestId, p.Index)),
                    Status = statuses.GetValueOrDefault((p.ContestId, p.Index), ProblemStatuses.None)
                });
        }
        return new LadderResponse(available, page.Select(g => new LadderGroup(
            ranked.Where(t => (bits[t.Name] & g.Key) != 0).Select(t => t.Name).ToArray(),
            query.IncludeProblems ? g.Where(r => tasks.ContainsKey((r.ContestId, r.Index)))
                .Select(r => tasks[(r.ContestId, r.Index)]).ToArray() : [], g.Count(),
            g.Count(r => solved.Contains((r.ContestId, r.Index))),
            g.Count(r => statuses.GetValueOrDefault((r.ContestId, r.Index)) == ProblemStatuses.Failed)))
            .ToArray(), groups.Length);
    }

    public async Task<LadderParticipantReport> GetParticipantsAsync(LadderQuery query, bool exactTags,
        CancellationToken cancellationToken)
    {
        var tags = query.Tags.Distinct(StringComparer.Ordinal).OrderBy(t => t, StringComparer.Ordinal).ToArray();
        var level = GetLevel(query);
        var problems = exactTags
            ? level.Where(p => p.Tags.Count == tags.Length && p.Tags.All(t => tags.Contains(t.Name)))
            : SelectTags(level, tags);
        var totalProblems = await problems.CountAsync(cancellationToken);
        var accepted = await (from s in db.Submissions
                              join p in problems on new { s.ContestId, s.Index } equals new { p.ContestId, p.Index }
                              where s.Verdict == Verdicts.Ok
                              group s by new { s.UserId, s.ContestId, s.Index } into g
                              select new
                              {
                                  g.Key.UserId,
                                  g.Key.ContestId,
                                  g.Key.Index,
                                  FirstAcceptedAt = g.Min(s => s.CreationTimeSeconds)
                              })
            .ToListAsync(cancellationToken);
        var solvedTasks = await problems.Where(p => db.Submissions.Any(s => s.Verdict == Verdicts.Ok &&
                s.ContestId == p.ContestId && s.Index == p.Index))
            .Select(p => new
            {
                p.ContestId,
                p.Index,
                p.Rating,
                p.Contest.Gym,
                p.Contest.Source,
                Name = p.ProblemTranslations.OrderByDescending(t => t.LanguageCode == query.Lang)
                    .ThenBy(t => t.LanguageCode).Select(t => t.Name).FirstOrDefault() ?? p.Index
            })
            .ToDictionaryAsync(p => (p.ContestId, p.Index), cancellationToken);
        var byUser = accepted.ToLookup(s => s.UserId);
        var users = await db.Profiles.AsNoTracking()
            .Select(u => new { u.Id, u.Handle, u.FirstName, u.LastName }).ToArrayAsync(cancellationToken);
        var participants = users.Select(user =>
        {
            var solved = byUser[user.Id].OrderBy(s => s.ContestId).ThenBy(s => s.Index, StringComparer.Ordinal)
                .Select(s =>
                {
                    var task = solvedTasks[(s.ContestId, s.Index)];
                    return new LadderSolvedProblem(task.ContestId, task.Index, task.Name, task.Rating,
                        task.Gym, task.Source, s.FirstAcceptedAt);
                }).ToArray();
            return new LadderParticipantResult(user.Handle, user.FirstName, user.LastName, solved.Length, solved);
        }).OrderByDescending(u => u.SolvedCount).ThenBy(u => u.Handle, StringComparer.Ordinal).ToArray();
        return new LadderParticipantReport(query.Division, query.Difficulty, query.Rating, tags,
            exactTags, totalProblems, participants);
    }

    public Task<List<LadderStatistic>> GetStatisticsAsync(LadderQuery query, CancellationToken cancellationToken)
    {
        var problems = SelectTags(GetLevel(query), query.Tags);
        var submissions = from s in db.Submissions
                          join p in problems on new { s.ContestId, s.Index } equals new { p.ContestId, p.Index }
                          select new { s.User.Handle, s.ContestId, s.Index, s.Verdict, s.CreationTimeSeconds };
        return submissions.GroupBy(s => new { s.Handle, s.ContestId, s.Index })
            .OrderBy(g => g.Key.Handle).ThenBy(g => g.Key.ContestId).ThenBy(g => g.Key.Index)
            .Select(g => new LadderStatistic(g.Key.Handle, g.Key.ContestId, g.Key.Index, g.Count(),
                g.Any(s => s.Verdict == Verdicts.Ok),
                g.Min(s => s.Verdict == Verdicts.Ok ? (long?)s.CreationTimeSeconds : null),
                g.Max(s => s.CreationTimeSeconds))).ToListAsync(cancellationToken);
    }
}
