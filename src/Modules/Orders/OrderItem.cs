using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Modules.Catalog;
using LedgerAI.Api.Modules.Inventory;

namespace LedgerAI.Api.Modules.Orders;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid OrderedUnitId { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal OrderedUnitPrice { get; set; }
    public decimal ConversionFactor { get; set; } = 1m;
    public decimal BaseQuantity { get; set; }
    public decimal UnitCostAtSale { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TotalCost { get; set; }

    // Navigation properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
    public StockBatch? Batch { get; set; }
    public UnitOfMeasure? OrderedUnit { get; set; }
}
