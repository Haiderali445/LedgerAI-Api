namespace LedgerAI.Api.Data.Dtos.AI;

public class NormalizedIntentDto
{
    public string IntentType { get; set; } = string.Empty; // e.g. "CreateOrder", "CheckBalance", "InquireStock"
    public decimal ConfidenceScore { get; set; }
    public string? CustomerIdentifier { get; set; }
    public IReadOnlyList<NormalizedIntentItemDto> Items { get; set; } = Array.Empty<NormalizedIntentItemDto>();
    public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    public string? RawText { get; set; }
}

public class NormalizedIntentItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? TargetPrice { get; set; }
}
