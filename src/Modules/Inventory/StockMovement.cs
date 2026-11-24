using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Core.Enums;
using LedgerAI.Api.Modules.Catalog;

namespace LedgerAI.Api.Modules.Inventory;

public class StockMovement : BaseEntity
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid BatchId { get; set; }
    public StockMovementType MovementType { get; set; } = StockMovementType.Purchase;
    public decimal QuantityDelta { get; set; }
    public decimal UnitCost { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public string? Notes { get; set; }
    public Guid? CreatedBy { get; set; }

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public Product? Product { get; set; }
    public StockBatch? Batch { get; set; }
}
