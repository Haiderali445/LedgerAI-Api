using LedgerAI.Api.Core.Enums;

namespace LedgerAI.Api.Data.Dtos.Orders;

public class CreateOrderDto
{
    public Guid AccountId { get; set; }
    public PaymentTerms PaymentTerms { get; set; } = PaymentTerms.Credit;
    public decimal UpfrontPaidAmount { get; set; }
    public string? RawIntentSummary { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid OrderedUnitId { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal OrderedUnitPrice { get; set; }
}
