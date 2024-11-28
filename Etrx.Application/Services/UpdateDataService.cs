using Etrx.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services
{
    public class UpdateDataService : BackgroundService, IUpdateDataService
    {
        private readonly ILogger<UpdateDataService> _logger;
        private readonly ICodeforcesApiService _externalApiService;
        private readonly ILastUpdateTimeService _lastTimeUpdateService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private DateTime _nextRunTime;

        public UpdateDataService(ILogger<UpdateDataService> logger,
                                 ICodeforcesApiService externalApiService,
                                 ILastUpdateTimeService lastTimeUpdateService,
                                 IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _externalApiService = externalApiService;
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

                bool success = false;
                try
                {
                    success = (await UpdateProblems()).Success;
                    success = (await UpdateContests()).Success;
                    success = (await UpdateUsers()).Success;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Task failed: {ex.Message}");
                    success = false;
                }

                if (success)
                {
                    _nextRunTime = CalculateNextRunTime();
                }
                else
                {
                    _nextRunTime = _nextRunTime.AddHours(1);
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

        public async Task<(bool Success, string Error)> UpdateProblems()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();

            var (Problems, ProblemStatistics, Error) = await _externalApiService.GetCodeforcesProblemsAsync();

            if (!string.IsNullOrEmpty(Error))
                return (false, Error);

            await codeforcesService.PostProblemsFromCodeforces(Problems!, ProblemStatistics!);

            _logger.LogInformation($"Problems updated successfully.");
            _lastTimeUpdateService.UpdateLastUpdateTime("problems", DateTime.Now.AddHours(3));
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)> UpdateContests()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();

            var (Contests, Error) = await _externalApiService.GetCodeforcesContestsAsync(false);

            if (!string.IsNullOrEmpty(Error))
                return (false, Error);

            await codeforcesService.PostContestsFromCodeforces(Contests!, false);

            (Contests, Error) = await _externalApiService.GetCodeforcesContestsAsync(true);

            if (!string.IsNullOrEmpty(Error))
                return (false, Error);

            await codeforcesService.PostContestsFromCodeforces(Contests!, true);

            _logger.LogInformation($"Contests updated successfully.");
            _lastTimeUpdateService.UpdateLastUpdateTime("contests", DateTime.Now.AddHours(3));
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)> UpdateUsers()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();

            var dlUsers = await _externalApiService.GetDlUsersAsync();

            if (!string.IsNullOrEmpty(dlUsers.Error))
                return (false, dlUsers.Error);

            string handlesString = string.Join(';', dlUsers.Users!.Select(user => user.Handle.ToLower()));
            var (Users, Error) = await _externalApiService.GetCodeforcesUsersAsync(handlesString);

            if (!string.IsNullOrEmpty(Error))
                return (false, Error);

            await codeforcesService.PostUsersFromDlCodeforces(dlUsers.Users!, Users!);

            _logger.LogInformation($"Dl users updated successfully.");
            _lastTimeUpdateService.UpdateLastUpdateTime("users", DateTime.Now.AddHours(3));
            return (true, string.Empty);
        }
    }
}
