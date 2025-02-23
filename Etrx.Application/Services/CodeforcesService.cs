using Etrx.Domain.Models.ParsingModels.Dl;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

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
                var dlUser = dlUsersList[i];

                if (user != null)
                {
                    var newUser = new User()
                    {
                        Handle = dlUser.Handle,
                        Email = user.Email,
                        VkId = user.VkId,
                        OpenId = user.OpenId,
                        FirstName = dlUser.FirstName,
                        LastName = dlUser.LastName,
                        Country = user.Country,
                        City = dlUser.City,
                        Organization = dlUser.Organization,
                        Contribution = user.Contribution,
                        Rank = user.Rank,
                        Rating = user.Rating,
                        MaxRank = user.MaxRank,
                        MaxRating = user.MaxRating,
                        LastOnlineTimeSeconds = user.LastOnlineTimeSeconds,
                        RegistrationTimeSeconds = user.RegistrationTimeSeconds,
                        FriendOfCount = user.FriendOfCount,
                        Avatar = user.Avatar,
                        TitlePhoto = user.TitlePhoto,
                        Grade = dlUser.Grade
                    };

                    users.Add(newUser);
                }
                else
                {
                    Console.WriteLine($"User {dlUser.Handle} doesn't exist in Codeforces");
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
                var newProblem = new Problem()
                {
                    ContestId = problem.ContestId,
                    Index = problem.Index,
                    Name = problem.Name,
                    Type = problem.Type,
                    Points = problem.Points,
                    Rating = problem.Rating,
                    SolvedCount = solvedCount,
                    Tags = problem.Tags
                };

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

                var newContest = new Contest()
                {
                    ContestId = contest.ContestId,
                    Name = contest.Name,
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
                    Gym = gym
                };

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
    }
}
