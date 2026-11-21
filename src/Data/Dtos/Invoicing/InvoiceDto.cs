using LedgerAI.Api.Core.Enums;

namespace LedgerAI.Api.Data.Dtos.Invoicing;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public string VoucherSeries { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public Guid AccountId { get; set; }
    public string InvoiceStatus { get; set; } = string.Empty;
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
}
