using Etrx.Application.Interfaces;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
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

    protected override async Task ProcessAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        var updateDataService = serviceProvider.GetRequiredService<IUpdateDataService>();
        var contestsRepository = serviceProvider.GetRequiredService<IContestsRepository>();

        var last10Contests = await contestsRepository
            .GetAll()
            .Where(c => c.Phase == "FINISHED" && !c.IsContestLoaded)
            .OrderByDescending(c => c.StartTime)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var contest in last10Contests)
        {
            await updateDataService.UpdateRanklistRowsByContestId(contest.ContestId);
            logger.LogInformation("Updated data for context {ContestId}.", contest.ContestId);

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