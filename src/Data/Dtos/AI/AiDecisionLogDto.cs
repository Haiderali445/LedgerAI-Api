namespace LedgerAI.Api.Data.Dtos.AI;

public class AiDecisionLogDto
{
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public string AgentName { get; set; } = string.Empty;
    public string InputPayload { get; set; } = string.Empty;
    public string OutputPayload { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public long LatencyMs { get; set; }
    public string ModelUsed { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
