using LedgerAI.Api.Data.Dtos.AI;

namespace LedgerAI.Api.AI.Monitoring;

public class AiDecisionLogger : IAiTelemetryService
{
    public Task LogDecisionAsync(AiDecisionLogDto decisionLog, CancellationToken cancellationToken = default)
    {
        // Stub: AI decision and observability logging
        return Task.CompletedTask;
    }
}
