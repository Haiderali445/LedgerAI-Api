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
        _logger.LogInformation("OutboxDispatcherWorker background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Stub: Outbox dispatch loop will poll and dispatch pending events
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
