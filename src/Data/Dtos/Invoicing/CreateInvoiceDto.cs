using LedgerAI.Api.Core.Enums;

namespace LedgerAI.Api.Data.Dtos.Invoicing;

public class CreateInvoiceDto
{
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Sale;
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? TransporterAccountId { get; set; }
    public string? BiltyNumber { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
    public List<CreateInvoiceChargeDto> Charges { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid BatchId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxRate { get; set; }
}

public class CreateInvoiceChargeDto
{
    public string ChargeType { get; set; } = "FREIGHT_BILTY";
    public ChargeDirection ChargeDirection { get; set; } = ChargeDirection.AddToBill;
    public AccountingTreatment AccountingTreatment { get; set; } = AccountingTreatment.FulfillmentExpense;
    public Guid? PayableAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
