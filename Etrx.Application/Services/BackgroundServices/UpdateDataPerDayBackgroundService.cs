using Etrx.Application.Interfaces;
using Etrx.Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services.BackgroundServices;

public class UpdateDataPerDayBackgroundService : UpdateDataBackgroundService
{
    public UpdateDataPerDayBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UpdateDataPerDayBackgroundService> logger)
        : base(serviceScopeFactory, logger)
    { }

    protected override async Task ProcessAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var updateDataService = serviceProvider.GetRequiredService<IUpdateDataService>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        var last10Contests = await unitOfWork.Contests.GetLast10Async();

        foreach (var contest in last10Contests)
        {
            await updateDataService.UpdateRanklistRowsByContestId(contest.ContestId);
            await Task.Delay(2000, cancellationToken);
        }
    }

    protected override TimeSpan CalculateNextDelay(bool executionSucceeded) 
    {
        if (executionSucceeded)
        {
            var now = DateTime.Now.AddHours(3);

            var nextRun = now.Date.AddHours(3);
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }
            return nextRun - now;
        }

        return TimeSpan.FromMinutes(30);
    }
}