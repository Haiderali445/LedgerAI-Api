using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Modules.Catalog;

namespace LedgerAI.Api.Modules.Inventory;

public class StockBatch : BaseEntity
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public DateOnly? ManufacturedAt { get; set; }
    public DateOnly? ExpiresAt { get; set; }

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public Product? Product { get; set; }
}
