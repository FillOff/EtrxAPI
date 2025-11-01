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
        var newUser = new User()
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

        await _usersRepository.InsertOrUpdateAsync([newUser]);
    }

    public async Task PostProblemsFromCodeforces(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics, string languageCode)
    {
        List<Problem> newProblems = [];
        List<ProblemTranslation> newTranslations = [];
        for (int i = 0; i < problems.Count; i++)
        {
            var solvedCount = problemStatistics.FirstOrDefault(s => s.ContestId == problems[i].ContestId && s.Index == problems[i].Index)!.SolvedCount;
            var newProblem = new Problem()
            {
                ContestId = problems[i].ContestId,
                Index = problems[i].Index,
                Type = problems[i].Type,
                Points = problems[i].Points,
                Rating = problems[i].Rating,
                SolvedCount = solvedCount,
                Tags = problems[i].Tags
            };

            var newProblemTranslation = new ProblemTranslation()
            {
                ContestId = problems[i].ContestId,
                Index = problems[i].Index,
                LanguageCode = languageCode,
                Name = problems[i].Name
            };

            newProblems.Add(newProblem);
            newTranslations.Add(newProblemTranslation);
        }

        await _problemsRepository.InsertOrUpdateAsync(newProblems);
        await _problemTranslationsRepository.InsertOrUpdateAsync(newTranslations);
    }

    public async Task PostContestsFromCodeforces(List<CodeforcesContest> contests, bool gym, string languageCode)
    {
        List<Contest> newContests = [];
        List<ContestTranslation> newTranslations = [];
        var existedContests = await _contestsRepository.GetAllAsync();

        for (int i = 0; i < contests.Count; i++)
        {
            var contest = contests[i];

            var newContest = new Contest()
            {
                ContestId = contest.ContestId,
                Type = contest.Type,
                Phase = contest.Phase,
                Frozen = contest.Frozen,
                DurationSeconds = contest.DurationSeconds,
                StartTime = contest.StartTime,
                RelativeTimeSeconds = contest.RelativeTimeSeconds,
                PreparedBy = contest.PreparedBy,
                WebsiteUrl = contest.WebsiteUrl,
                Description = contest.Description,
                Difficulty = contest.Difficulty,
                Kind = contest.Kind,
                IcpcRegion = contest.IcpcRegion,
                Country = contest.Country,
                City = contest.City,
                Season = contest.Season,
                Gym = gym,
                IsContestLoaded = existedContests.FirstOrDefault(c => c.ContestId == contest.ContestId)?.IsContestLoaded ?? false
            };

            var newContestTranslation = new ContestTranslation()
            {
                ContestId = contest.ContestId,
                LanguageCode = languageCode,
                Name = contest.Name
            };

            newContests.Add(newContest);
            newTranslations.Add(newContestTranslation);
        }

        await _contestsRepository.InsertOrUpdateAsync(newContests);
        await _contestTranslationsRepository.InsertOrUpdateAsync(newTranslations);
    }

    public async Task PostSubmissionsFromCodeforces(List<CodeforcesSubmission> submissions, string handle)
    {
        List<Submission> newSubmissions = [];
        for (int i = 0; i < submissions.Count; i++)
        {
            var submission = submissions[i];

            var newSubmission = new Submission()
            {
                Id = submission.Id,
                ContestId = submission.ContestId,
                Index = submission.Problem.Index,
                CreationTimeSeconds = submission.CreationTimeSeconds,
                RelativeTimeSeconds = submission.RelativeTimeSeconds,
                ProgrammingLanguage = submission.ProgrammingLanguage,
                Handle = handle,
                ParticipantType = submission.Author.ParticipantType,
                Verdict = submission.Verdict,
                Testset = submission.Testset,
                PassedTestCount = submission.PassedTestCount,
                TimeConsumedMillis = submission.TimeConsumedMillis,
                MemoryConsumedBytes = submission.MemoryConsumedBytes
            };

            newSubmissions.Add(newSubmission);
        }

        await _submissionsRepository.InsertOrUpdateAsync(newSubmissions);
    }

    public async Task PostRanklistRowsFromCodeforces(CodeforcesContestStanding contestStanding)
    {
        List<RanklistRow> newRows = [];
        for (int i = 0; i < contestStanding.Rows.Count; i++)
        {
            var row = contestStanding.Rows[i];

            var newRow = new RanklistRow()
            {
                Handle = row.Party.Members[0].Handle,
                ContestId = contestStanding.Contest.ContestId,
                ParticipantType = row.Party.ParticipantType,

                Rank = row.Rank,
                Points = row.Points,
                Penalty = row.Penalty,
                SuccessfulHackCount = row.SuccessfulHackCount,
                UnsuccessfulHackCount = row.UnsuccessfulHackCount,
                LastSubmissionTimeSeconds = row.LastSubmissionTimeSeconds
            };

            newRows.Add(newRow);
        }

        await _ranklistRowsRepository.InsertOrUpdateAsync(newRows);

        for (int i = 0; i < contestStanding.Rows.Count; i++)
        {
            await PostProblemResultsFromCodeforces(
                contestStanding.Rows[i].Party.Members[0].Handle,
                contestStanding.Contest.ContestId,
                contestStanding.Rows[i],
                contestStanding.Problems.Select(p => p.Index).ToList());
        }

        var contest = await _contestsRepository.GetByKeyAsync(contestStanding.Contest.ContestId);
        if (contest!.Phase == "FINISHED")
        { 
            contest.IsContestLoaded = true;
            await _contestsRepository.UpdateAsync(contest);
        }
    }

    public async Task PostProblemResultsFromCodeforces(string handle, int contestId, CodeforcesRanklistRow row, List<string> indexes)
    {
        List<ProblemResult> newProblemResults = [];

        for (int i = 0; i < row.ProblemResults.Count; i++)
        {
            var result = row.ProblemResults[i];

            var newProblemResult = new ProblemResult()
            {
                Handle = handle,
                ContestId = contestId,
                Index = indexes[i],
                ParticipantType = row.Party.ParticipantType,

                Points = result.Points,
                Penalty = result.Penalty,
                Type = result.Type,
                BestSubmissionTimeSeconds = result.BestSubmissionTimeSeconds,
                RejectedAttemptCount = result.RejectedAttemptCount
            };

            newProblemResults.Add(newProblemResult);
        }

        await _problemResultsRepository.InsertOrUpdateAsync(newProblemResults);
    }
}
