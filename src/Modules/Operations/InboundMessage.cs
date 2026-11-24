using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Core.Enums;

namespace LedgerAI.Api.Modules.Operations;

public class InboundMessage : BaseEntity
{
    public string Wamid { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public string Channel { get; set; } = "WhatsApp";
    public string RawPayloadJson { get; set; } = "{}";
    public string? AudioMediaId { get; set; }
    public string? TranscriptionText { get; set; }
    public InboundProcessingStatus ProcessingStatus { get; set; } = InboundProcessingStatus.Received;
    public Guid? MatchedOrderId { get; set; }
    public decimal? AiConfidenceScore { get; set; }
    public string? ErrorLog { get; set; }
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; set; }
}
