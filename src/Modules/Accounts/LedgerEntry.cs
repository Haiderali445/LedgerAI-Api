using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Accounts;

public class LedgerEntry : BaseEntity
{
    public Guid AccountId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? InvoiceId { get; set; }
    public string EntryType { get; set; } = "DEBIT"; // DEBIT or CREDIT
    public decimal Amount { get; set; }
    public decimal RunningBalance { get; set; }
    public string? Notes { get; set; }
    public string? IdempotencyKey { get; set; }
    public Guid? ReversesEntryId { get; set; }
    public Guid? CreatedBy { get; set; }

    // Navigation properties
    public Account? Account { get; set; }
    public LedgerEntry? ReversesEntry { get; set; }
}
