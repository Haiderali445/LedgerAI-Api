namespace LedgerAI.Api.Data.Dtos.Accounts;

public class PaymentAllocationDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid DebitEntryId { get; set; }
    public Guid CreditEntryId { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
