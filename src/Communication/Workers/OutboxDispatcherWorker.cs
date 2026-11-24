using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LedgerAI.Api.Communication.Workers;

public class OutboxDispatcherWorker : BackgroundService
{
    private readonly ILogger<OutboxDispatcherWorker> _logger;

    public OutboxDispatcherWorker(ILogger<OutboxDispatcherWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxDispatcherWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "OutboxDispatcherWorker polling cycle failed");
            }
        }

        _logger.LogInformation("OutboxDispatcherWorker stopped");
    }
}
