using LedgerAI.Api.Data.Dtos.AI;

namespace LedgerAI.Api.AI.Monitoring;

public interface IAiTelemetryService
{
    Task LogDecisionAsync(AiDecisionLogDto decisionLog, CancellationToken cancellationToken = default);
}
