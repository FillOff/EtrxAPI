using Etrx.Domain.Parsing_models;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;

namespace Etrx.Application.Services
{
    public class CodeforcesService : ICodeforcesService
    {
        private readonly IProblemsRepository _problemsRepository;
        private readonly IContestsRepository _contestsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ISubmissionsRepository _submissionsRepository;

        public CodeforcesService(
            IProblemsRepository problemsRepository,
            IContestsRepository contestsRepository,
            IUsersRepository usersRepository,
            ISubmissionsRepository submissionsRepository)
        {
            _problemsRepository = problemsRepository;
            _contestsRepository = contestsRepository;
            _usersRepository = usersRepository;
            _submissionsRepository = submissionsRepository;
        }

        public async Task PostUsersFromDlCodeforces(List<DlUser> dlUsersList, List<CodeforcesUser> codeforcesUsersList)
        {
            List<User> users = [];
            for (int i = 0; i < dlUsersList.Count; i++)
            {
                var user = codeforcesUsersList.FirstOrDefault(u => u.Handle.Equals(dlUsersList[i].Handle, StringComparison.CurrentCultureIgnoreCase));
                if (user != null)
                {
                    DateTime? lastTime = user.LastOnlineTimeSeconds != null 
                        ? DateTimeOffset.FromUnixTimeSeconds((long)user.LastOnlineTimeSeconds).UtcDateTime 
                        : null;
                    DateTime? regTime = user.RegistrationTimeSeconds != null 
                        ? DateTimeOffset.FromUnixTimeSeconds((long)user.RegistrationTimeSeconds).UtcDateTime 
                        : null;

                    var newUser = new User(
                        0,
                        dlUsersList[i].Handle,
                        user.Email,
                        user.VkId,
                        user.OpenId,
                        dlUsersList[i].FirstName,
                        dlUsersList[i].LastName,
                        user.Country,
                        dlUsersList[i].City,
                        dlUsersList[i].Organization,
                        user.Contribution,
                        user.Rank,
                        user.Rating,
                        user.MaxRank,
                        user.MaxRating,
                        lastTime,
                        regTime,
                        user.FriendOfCount,
                        user.Avatar,
                        user.TitlePhoto,
                        dlUsersList[i].Grade);

                    users.Add(newUser);
                }
                else
                {
                    Console.WriteLine($"User {dlUsersList[i].Handle} doesn't exist in Codeforces");
                }
            }
            await _usersRepository.InsertOrUpdateAsync(users);
        }

        public async Task PostProblemsFromCodeforces(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics)
        {
            List<Problem> newProblems = [];
            for (int i = 0; i < problems.Count; i++)
            {
                var problem = problems[i];
                var solvedCount = problemStatistics.FirstOrDefault(s => s.ContestId == problem.ContestId && s.Index == problem.Index)!.SolvedCount;
                var newProblem = new Problem(
                    0,
                    problem.ContestId,
                    problem.Index,
                    problem.Name,
                    problem.Type,
                    problem.Points,
                    problem.Rating,
                    solvedCount,
                    problem.Tags);
                newProblems.Add(newProblem);
            }
            await _problemsRepository.InsertOrUpdateAsync(newProblems);
        }

        public async Task PostContestsFromCodeforces(List<CodeforcesContest> contests, bool gym)
        {
            List<Contest> newContests = [];
            for (int i = 0; i < contests.Count; i++)
            {
                var contest = contests[i];
                DateTime? startTime = contest.StartTime != null 
                    ? DateTimeOffset.FromUnixTimeSeconds((long)contest.StartTime).UtcDateTime 
                    : null;
                DateTime? relativeTimeSeconds = contest.RelativeTimeSeconds != null 
                    ? DateTimeOffset.FromUnixTimeSeconds((long)contest.RelativeTimeSeconds).UtcDateTime 
                    : null;

                var newContest = new Contest(
                    contest.ContestId,
                    contest.Name,
                    contest.Type,
                    contest.Phase,
                    contest.Frozen,
                    contest.DurationSeconds,
                    startTime,
                    relativeTimeSeconds,
                    contest.PreparedBy,
                    contest.WebsiteUrl,
                    contest.Description,
                    contest.Difficulty,
                    contest.Kind,
                    contest.IcpcRegion,
                    contest.Country,
                    contest.City,
                    contest.Season,
                    gym);

                newContests.Add(newContest);
            }
            await _contestsRepository.InsertOrUpdateAsync(newContests);
        }

        public async Task PostSubmissionsFromCodeforces(List<CodeforcesSubmission> submissions, string handle)
        {
            List<Submission> newSubmissions = [];
            for (int i = 0; i < submissions.Count; i++)
            {
                var submission = submissions[i];
                DateTime creationTimeSeconds = DateTimeOffset.FromUnixTimeSeconds(submission.CreationTimeSeconds).UtcDateTime;
                DateTime relativeTimeSeconds = DateTimeOffset.FromUnixTimeSeconds(submission.RelativeTimeSeconds).UtcDateTime;

                var newSubmission = new Submission(
                    submission.Id,
                    submission.ContestId,
                    submission.Problem.Index,
                    creationTimeSeconds,
                    relativeTimeSeconds,
                    submission.ProgrammingLanguage,
                    handle,
                    submission.Author.ParticipantType,
                    submission.Verdict,
                    submission.Testset,
                    submission.PassedTestCount,
                    submission.TimeConsumedMillis,
                    submission.MemoryConsumedBytes);
                newSubmissions.Add(newSubmission);
            }
            await _submissionsRepository.InsertOrUpdateAsync(newSubmissions);
        }
    }
}
