using Etrx.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services.BackgroundServices;

public class UpdateDataEvery30MinutesBackgroundService : UpdateDataBackgroundService
{
    public UpdateDataEvery30MinutesBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UpdateDataEvery30MinutesBackgroundService> logger)
        : base(serviceScopeFactory, logger)
    { }

    protected override async Task ProcessAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        var updateDataService = serviceProvider.GetRequiredService<IUpdateDataService>();

        await updateDataService.UpdateContests();
        await updateDataService.UpdateProblems();
        await updateDataService.UpdateUsers();
        await updateDataService.UpdateSubmissions();
    }

    protected override TimeSpan CalculateNextDelay(bool executionSucceeded)
    {
        if (!executionSucceeded)
        {
            return TimeSpan.FromMinutes(5);
        }

        var now = DateTime.Now.AddHours(3);

        var nextRunMinute = (now.Minute / 30 + 1) * 30;
        var nextRunTime = now.Date.AddHours(now.Hour).AddMinutes(nextRunMinute);

        var delay = nextRunTime - now;

        if (delay <= TimeSpan.Zero)
        {
            delay = delay.Add(TimeSpan.FromMinutes(30));
        }

        return delay;
    }
}