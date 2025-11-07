using Etrx.Application.Interfaces;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;
using Etrx.Domain.Models.ParsingModels.Dl;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services;

public class CodeforcesService : ICodeforcesService
{
    private readonly IProblemsRepository _problemsRepository;
    private readonly IContestsRepository _contestsRepository;
    private readonly IGenericRepository<ContestTranslation, object> _contestTranslationsRepository;
    private readonly IGenericRepository<ProblemTranslation, object> _problemTranslationsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly IRanklistRowsRepository _ranklistRowsRepository;
    private readonly IGenericRepository<ProblemResult, object> _problemResultsRepository;

    public CodeforcesService(
        IProblemsRepository problemsRepository,
        IContestsRepository contestsRepository,
        IGenericRepository<ContestTranslation, object> contestTranslationsRepository,
        IGenericRepository<ProblemTranslation, object> problemTranslationsRepository,
        IUsersRepository usersRepository,
        ISubmissionsRepository submissionsRepository,
        ILogger<CodeforcesService> logger,
        IRanklistRowsRepository ranklistRowsRepository,
        IGenericRepository<ProblemResult, object> problemResultsRepository)
    {
        _problemsRepository = problemsRepository;
        _contestsRepository = contestsRepository;
        _contestTranslationsRepository = contestTranslationsRepository;
        _problemTranslationsRepository = problemTranslationsRepository;
        _usersRepository = usersRepository;
        _submissionsRepository = submissionsRepository;
        _ranklistRowsRepository = ranklistRowsRepository;
        _problemResultsRepository = problemResultsRepository;
    }

    public async Task PostUserFromDlCodeforces(DlUser dlUser, CodeforcesUser cfUser)
    {
        var existedUser = await _usersRepository.GetByHandleAsync(cfUser.Handle);

        User userEntity;

        if (existedUser is not null)
        {
            userEntity = existedUser;

            userEntity.Email = cfUser.Email;
            userEntity.VkId = cfUser.VkId;
            userEntity.OpenId = cfUser.OpenId;
            userEntity.FirstName = dlUser.FirstName;
            userEntity.LastName = dlUser.LastName;
            userEntity.Country = cfUser.Country;
            userEntity.City = dlUser.City;
            userEntity.Organization = dlUser.Organization;
            userEntity.Contribution = cfUser.Contribution;
            userEntity.Rank = cfUser.Rank;
            userEntity.Rating = cfUser.Rating;
            userEntity.MaxRank = cfUser.MaxRank;
            userEntity.MaxRating = cfUser.MaxRating;
            userEntity.LastOnlineTimeSeconds = cfUser.LastOnlineTimeSeconds;
            userEntity.RegistrationTimeSeconds = cfUser.RegistrationTimeSeconds;
            userEntity.FriendOfCount = cfUser.FriendOfCount;
            userEntity.Avatar = cfUser.Avatar;
            userEntity.TitlePhoto = cfUser.TitlePhoto;
            userEntity.Grade = dlUser.Grade;
        }
        else
        {
            userEntity = new User()
            {
                Handle = cfUser.Handle,
                Email = cfUser.Email,
                VkId = cfUser.VkId,
                OpenId = cfUser.OpenId,
                FirstName = dlUser.FirstName,
                LastName = dlUser.LastName,
                Country = cfUser.Country,
                City = dlUser.City,
                Organization = dlUser.Organization,
                Contribution = cfUser.Contribution,
                Rank = cfUser.Rank,
                Rating = cfUser.Rating,
                MaxRank = cfUser.MaxRank,
                MaxRating = cfUser.MaxRating,
                LastOnlineTimeSeconds = cfUser.LastOnlineTimeSeconds,
                RegistrationTimeSeconds = cfUser.RegistrationTimeSeconds,
                FriendOfCount = cfUser.FriendOfCount,
                Avatar = cfUser.Avatar,
                TitlePhoto = cfUser.TitlePhoto,
                Grade = dlUser.Grade
            };
        }

        await _usersRepository.InsertOrUpdateAsync([userEntity]);
    }

    public async Task PostProblemsFromCodeforces(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics, string languageCode)
    {
        var identifiers = problems
            .Select(p => new Tuple<int, string>(p.ContestId, p.Index))
            .ToList();

        var existingProblems = await _problemsRepository.GetAllAsync();
        var existingProblemsDict = existingProblems.ToDictionary(p => (p.ContestId, p.Index));

        var existingTranslations = await _problemTranslationsRepository.GetAllAsync();
        var existingTranslationsDict = existingTranslations.ToDictionary(pt => (pt.ProblemId, pt.LanguageCode));

        var statisticsDict = problemStatistics.ToDictionary(s => (s.ContestId, s.Index));

        List<Problem> problemsToUpsert = [];
        List<ProblemTranslation> translationsToUpsert = [];

        foreach (var incomingProblem in problems)
        {
            Guid problemId;
            Problem problemEntity;

            var problemKey = (incomingProblem.ContestId, incomingProblem.Index);
            statisticsDict.TryGetValue(problemKey, out var stats);
            var solvedCount = stats?.SolvedCount ?? 0;

            if (existingProblemsDict.TryGetValue(problemKey, out var existingProblem))
            {
                problemId = existingProblem.Id;
                problemEntity = existingProblem;

                problemEntity.Type = incomingProblem.Type;
                problemEntity.Points = incomingProblem.Points;
                problemEntity.Rating = incomingProblem.Rating;
                problemEntity.Tags = incomingProblem.Tags;
                problemEntity.SolvedCount = solvedCount;
            }
            else
            {
                problemId = Guid.NewGuid();
                problemEntity = new Problem
                {
                    Id = problemId,
                    ContestId = incomingProblem.ContestId,
                    Index = incomingProblem.Index,
                    Type = incomingProblem.Type,
                    Points = incomingProblem.Points,
                    Rating = incomingProblem.Rating,
                    Tags = incomingProblem.Tags,
                    SolvedCount = solvedCount
                };
            }

            problemsToUpsert.Add(problemEntity);

            ProblemTranslation problemTranslationEntity;
            if (existingTranslationsDict.TryGetValue((problemId, languageCode), out var existingTranslation))
            {
                existingTranslation.Name = incomingProblem.Name;
                problemTranslationEntity = existingTranslation;
            }
            else
            {
                problemTranslationEntity = new ProblemTranslation()
                {
                    ProblemId = problemId,
                    LanguageCode = languageCode,
                    Name = incomingProblem.Name,
                };
            }
            translationsToUpsert.Add(problemTranslationEntity);

        }

        await _problemsRepository.InsertOrUpdateAsync(problemsToUpsert);
        await _problemTranslationsRepository.InsertOrUpdateAsync(translationsToUpsert);
    }

    public async Task PostContestsFromCodeforces(List<CodeforcesContest> contests, bool gym, string languageCode)
    {
        var contestIdsFromApi = contests.Select(c => c.ContestId).ToList();

        var existingContests = await _contestsRepository.GetAllAsync();
        var existingContestsDict = existingContests.ToDictionary(c => c.ContestId);
        
        var existingTranslations = await _contestTranslationsRepository.GetAllAsync();
        var existingTranslationsDict = existingTranslations.ToDictionary(ct => (ct.ContestId, ct.LanguageCode));

        List<Contest> contestsToUpsert = [];
        List<ContestTranslation> translationsToUpsert = [];

        foreach (var incomingContest in contests)
        {
            Guid contestGuid;
            Contest contestEntity;

            if (existingContestsDict.TryGetValue(incomingContest.ContestId, out var existingContest))
            {
                contestGuid = existingContest.Id;
                contestEntity = existingContest;

                contestEntity.Type = incomingContest.Type;
                contestEntity.Phase = incomingContest.Phase;
                contestEntity.Frozen = incomingContest.Frozen;
                contestEntity.DurationSeconds = incomingContest.DurationSeconds;
                contestEntity.StartTime = incomingContest.StartTime;
                contestEntity.RelativeTimeSeconds = incomingContest.RelativeTimeSeconds;
                contestEntity.PreparedBy = incomingContest.PreparedBy;
                contestEntity.WebsiteUrl = incomingContest.WebsiteUrl;
                contestEntity.Description = incomingContest.Description;
                contestEntity.Difficulty = incomingContest.Difficulty;
                contestEntity.Kind = incomingContest.Kind;
                contestEntity.IcpcRegion = incomingContest.IcpcRegion;
                contestEntity.Country = incomingContest.Country;
                contestEntity.City = incomingContest.City;
                contestEntity.Season = incomingContest.Season;
                contestEntity.Gym = gym;

            }
            else
            {
                contestGuid = Guid.NewGuid();
                contestEntity = new Contest
                {
                    Id = contestGuid,
                    ContestId = incomingContest.ContestId,
                    Type = incomingContest.Type,
                    Phase = incomingContest.Phase,
                    Frozen = incomingContest.Frozen,
                    DurationSeconds = incomingContest.DurationSeconds,
                    StartTime = incomingContest.StartTime,
                    RelativeTimeSeconds = incomingContest.RelativeTimeSeconds,
                    PreparedBy = incomingContest.PreparedBy,
                    WebsiteUrl = incomingContest.WebsiteUrl,
                    Description = incomingContest.Description,
                    Difficulty = incomingContest.Difficulty,
                    Kind = incomingContest.Kind,
                    IcpcRegion = incomingContest.IcpcRegion,
                    Country = incomingContest.Country,
                    City = incomingContest.City,
                    Season = incomingContest.Season,
                    Gym = gym,
                    IsContestLoaded = false
                };
            }

            contestsToUpsert.Add(contestEntity);

            ContestTranslation contestTranslationEntity;
            if (existingTranslationsDict.TryGetValue((contestGuid, languageCode), out var existingTranslation))
            {
                existingTranslation.Name = incomingContest.Name;
                contestTranslationEntity = existingTranslation;
            }
            else
            {
                contestTranslationEntity = new ContestTranslation()
                {
                    ContestId = contestGuid,
                    LanguageCode = languageCode,
                    Name = incomingContest.Name
                };
            }

            translationsToUpsert.Add(contestTranslationEntity);
        }

        await _contestsRepository.InsertOrUpdateAsync(contestsToUpsert);
        await _contestTranslationsRepository.InsertOrUpdateAsync(translationsToUpsert);
    }

    public async Task PostSubmissionsFromCodeforces(List<CodeforcesSubmission> submissions, string handle)
    {
        var existingSubmissions = await _submissionsRepository.GetAllAsync();
        var existingSubmissionsDict = existingSubmissions.ToDictionary(s => s.SubmissionId);
        var user = await _usersRepository.GetByHandleAsync(handle)
            ?? throw new Exception($"User {handle} not found");

        List<Submission> submissionsToUpsert = [];

        foreach (var incomingubmission in submissions)
        {
            Submission submissionEntity;

            if (existingSubmissionsDict.TryGetValue(incomingubmission.Id, out var existingSubmission))
            {
                submissionEntity = existingSubmission;

                existingSubmission.ContestId = incomingubmission.ContestId;
                existingSubmission.Index = incomingubmission.Problem.Index;
                existingSubmission.CreationTimeSeconds = incomingubmission.CreationTimeSeconds;
                existingSubmission.RelativeTimeSeconds = incomingubmission.RelativeTimeSeconds;
                existingSubmission.ProgrammingLanguage = incomingubmission.ProgrammingLanguage;
                existingSubmission.UserId = user.Id;
                existingSubmission.ParticipantType = incomingubmission.Author.ParticipantType;
                existingSubmission.Verdict = incomingubmission.Verdict;
                existingSubmission.Testset = incomingubmission.Testset;
                existingSubmission.PassedTestCount = incomingubmission.PassedTestCount;
                existingSubmission.TimeConsumedMillis = incomingubmission.TimeConsumedMillis;
                existingSubmission.MemoryConsumedBytes = incomingubmission.MemoryConsumedBytes;
            }
            else
            {
                submissionEntity = new Submission()
                {
                    Id = Guid.NewGuid(),
                    SubmissionId = incomingubmission.Id,

                    ContestId = incomingubmission.ContestId,
                    Index = incomingubmission.Problem.Index,
                    CreationTimeSeconds = incomingubmission.CreationTimeSeconds,
                    RelativeTimeSeconds = incomingubmission.RelativeTimeSeconds,
                    ProgrammingLanguage = incomingubmission.ProgrammingLanguage,
                    UserId = user.Id,
                    ParticipantType = incomingubmission.Author.ParticipantType,
                    Verdict = incomingubmission.Verdict,
                    Testset = incomingubmission.Testset,
                    PassedTestCount = incomingubmission.PassedTestCount,
                    TimeConsumedMillis = incomingubmission.TimeConsumedMillis,
                    MemoryConsumedBytes = incomingubmission.MemoryConsumedBytes
                };
            }
            
            submissionsToUpsert.Add(submissionEntity);
        }

        await _submissionsRepository.InsertOrUpdateAsync(submissionsToUpsert);
    }

    public async Task PostRanklistRowsFromCodeforces(CodeforcesContestStanding contestStanding)
    {
        var contestId = contestStanding.Contest.ContestId;
        var problemIndexes = contestStanding.Problems.Select(p => p.Index).ToList();

        var existingRows = await _ranklistRowsRepository.GetByContestIdAsync(contestId);
        var existingRowsDict = existingRows.ToDictionary(rr => (rr.Handle, rr.ParticipantType));

        var existingRowGuids = existingRows.Select(rr => rr.Id).ToList();
        var existingProblemResults = await _problemResultsRepository.GetAllAsync();

        var existingProblemResultsDict = existingProblemResults
            .GroupBy(pr => pr.RanklistRowId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(pr => pr.Index)
            );

        List<RanklistRow> ranklistRowsToUpsert = [];
        List<ProblemResult> problemResultsToUpsert = [];

        foreach (var row in contestStanding.Rows)
        {
            var handle = row.Party.Members[0].Handle;
            Guid ranklistRowId;
            RanklistRow ranklistRowEntity;

            var rowKey = (handle, row.Party.ParticipantType);

            if (existingRowsDict.TryGetValue(rowKey, out var existingRow))
            {
                ranklistRowId = existingRow.Id;
                ranklistRowEntity = existingRow;

                ranklistRowEntity.Rank = row.Rank;
                ranklistRowEntity.Points = row.Points;
                ranklistRowEntity.Penalty = row.Penalty;
                ranklistRowEntity.SuccessfulHackCount = row.SuccessfulHackCount;
                ranklistRowEntity.UnsuccessfulHackCount = row.UnsuccessfulHackCount;
                ranklistRowEntity.LastSubmissionTimeSeconds = row.LastSubmissionTimeSeconds;
            }
            else
            {
                ranklistRowId = Guid.NewGuid();
                ranklistRowEntity = new RanklistRow
                {
                    Id = ranklistRowId,
                    Handle = handle,
                    ContestId = contestId,
                    ParticipantType = row.Party.ParticipantType,

                    Rank = row.Rank,
                    Points = row.Points,
                    Penalty = row.Penalty,
                    SuccessfulHackCount = row.SuccessfulHackCount,
                    UnsuccessfulHackCount = row.UnsuccessfulHackCount,
                    LastSubmissionTimeSeconds = row.LastSubmissionTimeSeconds
                };
            }

            ranklistRowsToUpsert.Add(ranklistRowEntity);

            for (int i = 0; i < row.ProblemResults.Count; i++)
            {
                var result = row.ProblemResults[i];
                var problemIndex = problemIndexes[i];
                ProblemResult problemResultEntity;

                if (existingProblemResultsDict.TryGetValue(ranklistRowId, out var resultsForThisRow) &&
                    resultsForThisRow.TryGetValue(problemIndex, out var existingResult))
                {
                    problemResultEntity = existingResult;

                    problemResultEntity.Points = result.Points;
                    problemResultEntity.Penalty = result.Penalty;
                    problemResultEntity.Type = result.Type;
                    problemResultEntity.BestSubmissionTimeSeconds = result.BestSubmissionTimeSeconds;
                    problemResultEntity.RejectedAttemptCount = result.RejectedAttemptCount;
                }
                else
                {
                    problemResultEntity = new ProblemResult
                    {
                        Id = Guid.NewGuid(),
                        RanklistRowId = ranklistRowId,
                        Index = problemIndex,
                        Points = result.Points,
                        Penalty = result.Penalty,
                        Type = result.Type,
                        BestSubmissionTimeSeconds = result.BestSubmissionTimeSeconds,
                        RejectedAttemptCount = result.RejectedAttemptCount
                    };
                }
                problemResultsToUpsert.Add(problemResultEntity);
            }
        }

        await _ranklistRowsRepository.InsertOrUpdateAsync(ranklistRowsToUpsert);
        await _problemResultsRepository.InsertOrUpdateAsync(problemResultsToUpsert);

        var contest = await _contestsRepository.GetByKeyAsync(contestId);
        if (contest!.Phase == "FINISHED")
        {
            contest.IsContestLoaded = true;
            await _contestsRepository.UpdateAsync(contest);
        }
    }
}