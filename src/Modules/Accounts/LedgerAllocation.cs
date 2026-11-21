using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Accounts;

public class LedgerAllocation : BaseEntity
{
    public Guid AccountId { get; set; }
    public Guid DebitEntryId { get; set; }
    public Guid CreditEntryId { get; set; }
    public decimal Amount { get; set; }
    public Guid? CreatedBy { get; set; }

    // Navigation properties
    public Account? Account { get; set; }
    public LedgerEntry? DebitEntry { get; set; }
    public LedgerEntry? CreditEntry { get; set; }
}
