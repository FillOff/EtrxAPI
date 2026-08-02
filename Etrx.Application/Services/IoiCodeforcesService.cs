using Etrx.Application.Constants;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Domain.Models;
using Etrx.Domain.Models.Parsing_models.IoiCodeforces;
using System.Globalization;

namespace Etrx.Application.Services;

public class IoiCodeforcesService : IIoiCodeforcesService
{
    private readonly IUnitOfWork _unitOfWork;

    public IoiCodeforcesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task PostContestsAsync(List<IoiCodeforcesContest> contests, string languageCode)
    {
        var contestIds = contests
            .Select(contest => ParseContestId(contest.Id))
            .ToList();
        var existingContests = await _unitOfWork.Contests.GetByContestIdsAsync(contestIds);
        var existingById = existingContests.ToDictionary(contest => contest.ContestId);
        var existingTranslations = await _unitOfWork.ContestTranslations
            .GetByContestIdsAndLanguageAsync(existingContests.Select(contest => contest.Id).ToList(), languageCode);
        var translationsByContestId = existingTranslations.ToDictionary(translation => translation.ContestId);

        var contestsToUpsert = new List<Contest>();
        var translationsToUpsert = new List<ContestTranslation>();

        foreach (var incomingContest in contests)
        {
            var contestId = ParseContestId(incomingContest.Id);
            if (!existingById.TryGetValue(contestId, out var contestEntity))
            {
                contestEntity = new Contest
                {
                    Id = Guid.NewGuid(),
                    ContestId = contestId,
                    Gym = true,
                    IsContestLoaded = false
                };
            }

            contestEntity.ContestId = contestId;
            contestEntity.Type = "IOI";
            contestEntity.Phase = "FINISHED";
            contestEntity.Gym = true;
            contestEntity.Source = "IOI";
            contestEntity.DurationSeconds = ParseDuration(incomingContest.Duration);
            contestEntity.StartTime = ParseStartTime(incomingContest.StartTime);
            contestsToUpsert.Add(contestEntity);

            if (!translationsByContestId.TryGetValue(contestEntity.Id, out var translation))
            {
                translation = new ContestTranslation
                {
                    Id = Guid.NewGuid(),
                    ContestId = contestEntity.Id,
                    LanguageCode = languageCode
                };
            }

            translation.Name = incomingContest.Name;
            translationsToUpsert.Add(translation);
        }

        await _unitOfWork.Contests.InsertOrUpdateAsync(contestsToUpsert);
        await _unitOfWork.ContestTranslations.InsertOrUpdateAsync(translationsToUpsert);
    }

    public async Task PostRanklistRowsAsync(int contestId, IoiCodeforcesStandings standings)
    {
        var contest = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new Exception($"Contest {contestId} not found");
        var existingRows = await _unitOfWork.RanklistRows.GetByContestIdAsync(contestId);
        var rowsByHandle = existingRows.ToDictionary(row => row.Handle);
        var existingResults = await _unitOfWork.ProblemResults
            .GetByRanklistRowIdsAsync(existingRows.Select(row => row.Id).ToList());
        var resultsByRow = existingResults
            .GroupBy(result => result.RanklistRowId)
            .ToDictionary(group => group.Key, group => group.ToDictionary(result => result.Index));

        var rowsToUpsert = new List<RanklistRow>();
        var resultsToUpsert = new List<ProblemResult>();
        var problemIndexes = standings.Rows
            .SelectMany(row => row.ProblemScores)
            .Where(result => !string.IsNullOrWhiteSpace(result.Index))
            .GroupBy(result => result.Index)
            .Select(group => group.First())
            .ToList();
        var existingProblems = await _unitOfWork.Problems.GetByContestAndIndexAsync(
            problemIndexes.Select(result => (contestId, result.Index)).ToList());
        var existingProblemsByIndex = existingProblems.ToDictionary(problem => problem.Index);
        var existingTranslations = await _unitOfWork.ProblemTranslations.GetByProblemIdsAndLanguageAsync(
            existingProblems.Select(problem => problem.Id).ToList(), "en");
        var translationsByProblemId = existingTranslations.ToDictionary(translation => translation.ProblemId);

        foreach (var incomingRow in standings.Rows)
        {
            if (string.IsNullOrWhiteSpace(incomingRow.Handle))
                continue;

            if (!rowsByHandle.TryGetValue(incomingRow.Handle, out var rowEntity))
            {
                rowEntity = new RanklistRow { Id = Guid.NewGuid(), ContestId = contestId, Handle = incomingRow.Handle };
            }

            rowEntity.Rank = ParseInt(incomingRow.Rank);
            rowEntity.Points = incomingRow.TotalScore;
            rowEntity.ParticipantType = ParticipantTypes.Practice;
            rowsToUpsert.Add(rowEntity);

            resultsByRow.TryGetValue(rowEntity.Id, out var rowResults);
            foreach (var incomingResult in incomingRow.ProblemScores)
            {
                if (string.IsNullOrWhiteSpace(incomingResult.Index))
                    continue;

                if (rowResults is null || !rowResults.TryGetValue(incomingResult.Index, out var resultEntity))
                {
                    resultEntity = new ProblemResult
                    {
                        Id = Guid.NewGuid(),
                        RanklistRowId = rowEntity.Id,
                        Index = incomingResult.Index
                    };
                }

                resultEntity.Points = incomingResult.Points;
                resultsToUpsert.Add(resultEntity);
            }
        }

        var problemsToUpsert = new List<Problem>();
        var problemTranslationsToUpsert = new List<ProblemTranslation>();
        foreach (var incomingProblem in problemIndexes)
        {
            if (!existingProblemsByIndex.TryGetValue(incomingProblem.Index, out var problemEntity))
            {
                problemEntity = new Problem
                {
                    Id = Guid.NewGuid(),
                    ContestId = contestId,
                    GuidContestId = contest.Id,
                    Index = incomingProblem.Index,
                    Type = "PROGRAMMING"
                };
            }

            problemEntity.ContestId = contestId;
            problemEntity.GuidContestId = contest.Id;
            problemEntity.Points = incomingProblem.Points;
            problemsToUpsert.Add(problemEntity);

            if (!translationsByProblemId.TryGetValue(problemEntity.Id, out var translation))
            {
                translation = new ProblemTranslation
                {
                    Id = Guid.NewGuid(),
                    ProblemId = problemEntity.Id,
                    LanguageCode = "en"
                };
            }

            translation.Name = $"IOI {incomingProblem.Index}";
            problemTranslationsToUpsert.Add(translation);
        }

        await _unitOfWork.RanklistRows.InsertOrUpdateAsync(rowsToUpsert);
        await _unitOfWork.ProblemResults.InsertOrUpdateAsync(resultsToUpsert);
        await _unitOfWork.Problems.InsertOrUpdateAsync(problemsToUpsert);
        await _unitOfWork.ProblemTranslations.InsertOrUpdateAsync(problemTranslationsToUpsert);

        contest.IsContestLoaded = true;
        _unitOfWork.Contests.Update(contest);
        await _unitOfWork.SaveAsync();
    }


    private static int ParseContestId(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
            ? id
            : throw new FormatException($"Invalid IOI contest id: {value}");

    private static int ParseInt(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) ? number : 0;

    private static int ParseDuration(string value)
    {
        return TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var duration)
            ? (int)duration.TotalSeconds
            : 0;
    }

    private static long ParseStartTime(string value)
    {
        return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var startTime)
            ? startTime.ToUnixTimeSeconds()
            : 0;
    }
}
