using Etrx.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services;

public class UpdateDataBackgroundService : BackgroundService
{
    private readonly ILogger<UpdateDataBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private DateTime _nextRunTime;
    private DateTime _nowMsk;

    public UpdateDataBackgroundService(
        ILogger<UpdateDataBackgroundService> logger, 
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UpdateDataService is starting.");

        using var scope = _serviceScopeFactory.CreateScope();
        var updateDataService = scope.ServiceProvider.GetRequiredService<IUpdateDataService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            _nowMsk = DateTime.Now.AddHours(3);
            var delay = _nextRunTime - _nowMsk;

            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            _logger.LogInformation($"Next execution scheduled at {_nextRunTime}.");

            await Task.Delay(delay, stoppingToken);

            try
            {
                await updateDataService.UpdateContests();
                await updateDataService.UpdateProblems();
                await updateDataService.UpdateUsers();
                await updateDataService.UpdateSubmissions();
                await Task.Delay(2000, stoppingToken);

                _nowMsk = DateTime.Now.AddHours(3);
                _nextRunTime = CalculateNextRunTime(_nowMsk);
            }
            catch (Exception ex)
            {
                _nextRunTime = CalculateNextRunTime(DateTime.Now.AddHours(3));

                _logger.LogWarning($"Task failed: {ex.Message}");
                _logger.LogWarning($"Task failed, rescheduled to {_nextRunTime}");
            }
        }

        _logger.LogInformation("UpdateDataService is stopping.");
    }

    private static DateTime CalculateNextRunTime(DateTime nowMsk)
    {
        var minutes = nowMsk.Minute;
        var nextRunMinute = (minutes / 30 + 1) * 30;

        var nextRunTime = nowMsk.Date.AddHours(nowMsk.Hour).AddMinutes(nextRunMinute);

        if (nextRunTime <= nowMsk)
        {
            nextRunTime = nextRunTime.AddMinutes(30);
        }

        return nextRunTime;
    }
}