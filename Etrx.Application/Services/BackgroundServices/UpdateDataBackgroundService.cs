using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services.BackgroundServices;

public abstract class UpdateDataBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger _logger;

    protected UpdateDataBackgroundService(
        IServiceScopeFactory serviceScopeFactory, 
        ILogger logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected abstract Task ProcessAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
    protected abstract TimeSpan CalculateNextDelay(bool executionSucceeded);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{ServiceName} is starting.", GetType().Name);
        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var executionSucceeded = false;

            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();

                await ProcessAsync(scope.ServiceProvider, stoppingToken);

                executionSucceeded = true;
                _logger.LogInformation("{ServiceName} finished task successfully.", GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {ServiceName}.", GetType().Name);
                executionSucceeded = false;
            }

            var delay = CalculateNextDelay(executionSucceeded);
            var nextRunTime = DateTime.Now.AddHours(3) + delay;

            _logger.LogInformation("{ServiceName} is waiting for the next run at {NextRunTime}. Delay is {Delay}",
                GetType().Name, nextRunTime, delay);

            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("{ServiceName} is stopping.", GetType().Name);
    }
}
