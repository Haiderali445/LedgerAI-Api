namespace LedgerAI.Api.Data.Dtos.Accounts;

public class LedgerEntryDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? InvoiceId { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal RunningBalance { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
