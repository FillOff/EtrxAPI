using Etrx.Application.Constants;
using Etrx.Application.Dtos.ProblemResults;
using Etrx.Application.Dtos.RanklistRows;
using Etrx.Application.Queries;
using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Application.Services;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class RanklistRowsRepository : GenericRepository<RanklistRow>, IRanklistRowsRepository
{
    public RanklistRowsRepository(EtrxDbContext context)
        : base(context)
    { }

    public override async Task<List<RanklistRow>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rr => rr.ProblemResults)
            .ToListAsync();
    }

    public async Task<List<RanklistRow>> GetByContestIdAsync(int contestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rr => rr.ProblemResults)
            .Where(rr => rr.ContestId == contestId)
            .ToListAsync();
    }

    public async Task<List<GetRanklistRowsResponseDto>> GetByContestIdWithSortAndFilterAsync(RanklistQueryParameters parameters)
    {
        var query = _context.RanklistRows
           .AsNoTracking()
           .Include(rr => rr.ProblemResults)
           .Where(rr => rr.ContestId == parameters.ContestId);

        // Filter by participant type
        if (parameters.ParticipantType != ParticipantTypes.All)
        {
            query = query.Where(rr => rr.ParticipantType == parameters.ParticipantType);
        }

        var rows = await query.ToListAsync();
        var memberHandles = rows
            .SelectMany(ranklistRow => ranklistRow.MemberHandles.Count > 0
                ? ranklistRow.MemberHandles
                : [ranklistRow.Handle])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(user => memberHandles.Contains(user.Handle))
            .ToListAsync();
        var usersByHandle = users.ToDictionary(user => user.Handle, StringComparer.OrdinalIgnoreCase);

        var combinedQuery = rows.Select(ranklistRow =>
        {
            var handles = ranklistRow.MemberHandles.Count > 0
                ? ranklistRow.MemberHandles
                : [ranklistRow.Handle];
            usersByHandle.TryGetValue(handles[0], out var primaryUser);
            var (organization, city) = RanklistPartyFormatter.GetOrganizationAndCity(ranklistRow, primaryUser ?? new User());

            return new GetRanklistRowsResponseDto
            {
                ContestId = ranklistRow.ContestId,
                Handle = ranklistRow.Handle,
                LastSubmissionTimeSeconds = ranklistRow.LastSubmissionTimeSeconds,
                ParticipantType = ranklistRow.ParticipantType,
                Penalty = ranklistRow.Penalty,
                Points = ranklistRow.Points,
                ProblemResults = ranklistRow.ProblemResults
                    .OrderBy(p => p.Index)
                    .Select(p => new GetProblemResultsResponseDto(p.Index, p.Points, p.Penalty, p.RejectedAttemptCount, p.Type, p.BestSubmissionTimeSeconds))
                    .ToList(),
                Rank = ranklistRow.Rank,
                SuccessfulHackCount = ranklistRow.SuccessfulHackCount,
                UnsuccessfulHackCount = ranklistRow.UnsuccessfulHackCount,
                Username = RanklistPartyFormatter.FormatName(ranklistRow.TeamName, handles, users),
                City = city,
                Organization = organization,
                Grade = primaryUser?.Grade ?? 0,
                SolvedCount = ranklistRow.ProblemResults.Count(pr => pr.Points != 0)
            };
        }).AsQueryable();

        // Sorting
        string order = parameters.Sorting.SortOrder == true ? SortOrders.Asc : SortOrders.Desc;
        combinedQuery = combinedQuery.OrderBy($"{parameters.Sorting.SortField} {order}");

        return combinedQuery.ToList();
    }
}
