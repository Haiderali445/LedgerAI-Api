using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Core.Enums;
using LedgerAI.Api.Modules.Accounts;
using LedgerAI.Api.Modules.Orders;

namespace LedgerAI.Api.Modules.Invoicing;

public class Invoice : BaseEntity
{
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Sale;
    public string VoucherSeries { get; set; } = "01"; // 00 for Purchase, 01 for Sale
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? TransporterAccountId { get; set; }
    public string? BiltyNumber { get; set; }
    public string InvoiceStatus { get; set; } = "ISSUED"; // DRAFT, ISSUED, PARTIALLY_PAID, PAID, VOID
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public DateOnly? DueDate { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? IssuedBy { get; set; }
    public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public Order? Order { get; set; }
    public Account? Account { get; set; }
    public Account? TransporterAccount { get; set; }
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<InvoiceCharge> Charges { get; set; } = new List<InvoiceCharge>();
}
