namespace LedgerAI.Api.Data.Dtos.Inventory;

public class StockBatchDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable => QuantityOnHand - QuantityReserved;
    public DateOnly? ExpiresAt { get; set; }
}
