using Etrx.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services
{
    public class UpdateDataService : BackgroundService
    {
        private readonly ILogger<UpdateDataService> _logger;
        private readonly IExternalApiService _externalApiService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private DateTime _nextRunTime;

        public UpdateDataService(ILogger<UpdateDataService> logger, 
                                 IExternalApiService externalApiService, 
                                 IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _externalApiService = externalApiService;
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

                using var scope = _serviceScopeFactory.CreateScope();

                var _codeforcesService = scope.ServiceProvider.GetRequiredService<ICodeforcesService>();

                bool success = false;
                success = await UpdateProblems(_codeforcesService);
                success = await UpdateContests(_codeforcesService);
                success = await UpdateUsers(_codeforcesService);

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

        protected async Task<bool> UpdateProblems(ICodeforcesService codeforcesService)
        {
            var (Problems, ProblemStatistics, Error) = await _externalApiService.GetCodeforcesProblemsAsync();

            if (!string.IsNullOrEmpty(Error))
                return false;

            await codeforcesService.PostProblemsFromCodeforces(Problems!, ProblemStatistics!);

            _logger.LogInformation($"Problems updated successfully.");
            return true;
        }

        protected async Task<bool> UpdateContests(ICodeforcesService codeforcesService)
        {
            var (Contests, Error) = await _externalApiService.GetCodeforcesContestsAsync(false);

            if (!string.IsNullOrEmpty(Error))
                return false;

            await codeforcesService.PostContestsFromCodeforces(Contests!, false);

            (Contests, Error) = await _externalApiService.GetCodeforcesContestsAsync(true);

            if (!string.IsNullOrEmpty(Error))
                return false;

            await codeforcesService.PostContestsFromCodeforces(Contests!, true);

            _logger.LogInformation($"Contests updated successfully.");
            return true;
        }

        protected async Task<bool> UpdateUsers(ICodeforcesService codeforcesService)
        {
            var dlUsers = await _externalApiService.GetDlUsersAsync();

            if (!string.IsNullOrEmpty(dlUsers.Error))
                return false;

            string handlesString = string.Join(';', dlUsers.Users!.Select(user => user.Handle.ToLower()));
            var (Users, Error) = await _externalApiService.GetCodeforcesUsersAsync(handlesString);

            if (!string.IsNullOrEmpty(Error))
                return false;

            await codeforcesService.PostUsersFromDlCodeforces(dlUsers.Users!, Users!);

            _logger.LogInformation($"Dl users updated successfully.");
            return true;
        }
    }
}
