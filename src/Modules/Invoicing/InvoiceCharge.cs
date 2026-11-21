using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Core.Enums;
using LedgerAI.Api.Modules.Accounts;

namespace LedgerAI.Api.Modules.Invoicing;

public class InvoiceCharge : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string ChargeType { get; set; } = "FREIGHT_BILTY";
    public ChargeDirection ChargeDirection { get; set; } = ChargeDirection.AddToBill;
    public AccountingTreatment AccountingTreatment { get; set; } = AccountingTreatment.FulfillmentExpense;
    public Guid? PayableAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Invoice? Invoice { get; set; }
    public Account? PayableAccount { get; set; }
}
