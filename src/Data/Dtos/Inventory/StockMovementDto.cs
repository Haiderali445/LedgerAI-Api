namespace LedgerAI.Api.Data.Dtos.Inventory;

public class StockMovementDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid BatchId { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public decimal QuantityDelta { get; set; }
    public decimal UnitCost { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
