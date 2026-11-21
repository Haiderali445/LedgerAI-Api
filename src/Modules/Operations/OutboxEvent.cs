using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Operations;

public class OutboxEvent : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string DedupeKey { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
    public int AttemptCount { get; set; } = 0;
    public DateTimeOffset AvailableAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LockedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public string? LastError { get; set; }
}
