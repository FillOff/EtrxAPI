using Etrx.Application.Interfaces;
using Etrx.Application.Constants;
using Etrx.Application.Repositories.UnitOfWork;
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

    private const int MaxSyncAttempts = 5;

    protected override async Task ProcessAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var updateDataService = serviceProvider.GetRequiredService<IUpdateDataService>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        var last10Contests = await unitOfWork.Contests.GetLast10Async();

        foreach (var contest in last10Contests)
        {
            try
            {
                if (contest.Source == Sources.Ioi)
                {
                    await updateDataService.UpdateIoiRanklistRowsByContestIdAsync(contest.ContestId);
                }
                else
                {
                    await updateDataService.UpdateRanklistRowsByContestIdAsync(contest.ContestId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update contest {ContestId}, skipping it", contest.ContestId);

                await unitOfWork.Contests.IncrementSyncAttemptsAsync(contest.ContestId);

                if (contest.SyncAttempts + 1 > MaxSyncAttempts)
                {
                    _logger.LogWarning(
                        "Contest {ContestId} failed {MaxSyncAttempts} times, marking it as loaded and never retrying it",
                        contest.ContestId, MaxSyncAttempts);

                    await unitOfWork.Contests.MarkAsLoadedAsync(contest.ContestId);
                }
            }

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