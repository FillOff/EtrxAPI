using Etrx.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services
{
    public class UpdateDataService : BackgroundService, IUpdateDataService
    {
        private readonly ILogger<UpdateDataService> _logger;
        private readonly ILastUpdateTimeService _lastTimeUpdateService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private DateTime _nextRunTime;

        public UpdateDataService(
            ILogger<UpdateDataService> logger,
            ILastUpdateTimeService lastTimeUpdateService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _lastTimeUpdateService = lastTimeUpdateService;
            _serviceScopeFactory = serviceScopeFactory;
            _nextRunTime = CalculateNextRunTime();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdateDataService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var nowMsk = DateTime.Now.AddHours(3);
                var delay = _nextRunTime - nowMsk;

                if (delay < TimeSpan.Zero)
                {
                    delay = TimeSpan.Zero;
                }

                _logger.LogInformation($"Next execution scheduled at {_nextRunTime}.");

                await Task.Delay(delay, stoppingToken);

                try
                {
                    await UpdateProblems();
                    await UpdateContests();
                    await UpdateUsers();

                    _nextRunTime = CalculateNextRunTime();
                }
                catch (Exception ex)
                {
                    _nextRunTime = _nextRunTime.AddHours(1);

                    _logger.LogWarning($"Task failed: {ex.Message}");
                    _logger.LogWarning($"Task failed, rescheduled to {_nextRunTime}");
                }
            }

            _logger.LogInformation("UpdateDataService is stopping.");
        }

        private DateTime CalculateNextRunTime()
        {
            var targetTime = DateTime.Today.AddHours(23).AddMinutes(00);

            if (DateTime.Now.AddHours(3) > targetTime)
            {
                targetTime = targetTime.AddDays(1);
            }

            return targetTime;
        }

        public async Task UpdateProblems()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();
            var codeforcesApiService = scope.ServiceProvider.GetRequiredService<ICodeforcesApiService>();

            var (Problems, ProblemStatistics) = await codeforcesApiService.GetCodeforcesProblemsAsync("ru");
            await codeforcesService.PostProblemsFromCodeforces(Problems!, ProblemStatistics!, "ru");

            (Problems, ProblemStatistics) = await codeforcesApiService.GetCodeforcesProblemsAsync("en");
            await codeforcesService.PostProblemsFromCodeforces(Problems!, ProblemStatistics!, "en");

            _logger.LogInformation($"Problems updated successfully.");
            _lastTimeUpdateService.UpdateLastUpdateTime("problems", DateTime.Now.AddHours(3));
        }

        public async Task UpdateContests()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();
            var codeforcesApiService = scope.ServiceProvider.GetRequiredService<ICodeforcesApiService>();

            var contests = await codeforcesApiService.GetCodeforcesContestsAsync(false, "ru");
            await codeforcesService.PostContestsFromCodeforces(contests!, false, "ru");

            contests = await codeforcesApiService.GetCodeforcesContestsAsync(true, "ru");
            await codeforcesService.PostContestsFromCodeforces(contests!, true, "ru");

            contests = await codeforcesApiService.GetCodeforcesContestsAsync(false, "en");
            await codeforcesService.PostContestsFromCodeforces(contests!, false, "en");

            contests = await codeforcesApiService.GetCodeforcesContestsAsync(true, "en");
            await codeforcesService.PostContestsFromCodeforces(contests!, true, "en");

            _logger.LogInformation($"Contests updated successfully.");
            _lastTimeUpdateService.UpdateLastUpdateTime("contests", DateTime.Now.AddHours(3));
        }

        public async Task UpdateUsers()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();
            var codeforcesApiService = scope.ServiceProvider.GetRequiredService<ICodeforcesApiService>();
            var dlApiService = scope.ServiceProvider.GetRequiredService<IDlApiService>();

            var dlUsers = await dlApiService.GetDlUsersAsync();
            foreach ( var dlUser in dlUsers )
            {
                var handle = dlUser.Handle;
                var user = await codeforcesApiService.GetCodeforcesUsersAsync(handle);

                await codeforcesService.PostUserFromDlCodeforces(dlUser, user[0]);
                await Task.Delay(2000);
            }

            _logger.LogInformation($"Dl users updated successfully.");
            _lastTimeUpdateService.UpdateLastUpdateTime("users", DateTime.Now.AddHours(3));
        }

        public async Task UpdateSubmissions()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();
            var codeforcesApiService = scope.ServiceProvider.GetRequiredService<ICodeforcesApiService>();
            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            var handles = await usersService.GetHandlesAsync();
            foreach (var handle in handles)
            {
                var submissions = await codeforcesApiService.GetCodeforcesSubmissionsAsync(handle);
                await codeforcesService.PostSubmissionsFromCodeforces(submissions, handle);
            }

            _logger.LogInformation($"Submissions updated successfully.");
        }

        public async Task UpdateSubmissionsByContestId(int contestId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();
            var codeforcesApiService = scope.ServiceProvider.GetRequiredService<ICodeforcesApiService>();
            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            var handles = await codeforcesApiService.GetCodeforcesContestUsersAsync(await usersService.GetHandlesAsync(), contestId);
            foreach(var handle in handles)
            {
                var submissions = await codeforcesApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);
                await codeforcesService.PostSubmissionsFromCodeforces(submissions, handle);
            }

            _logger.LogInformation($"Submissions updated successfully.");
        }
    }
}
