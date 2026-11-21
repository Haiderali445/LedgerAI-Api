using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Modules.Catalog;

namespace LedgerAI.Api.Modules.Inventory;

public class StockItem : BaseEntity
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public Product? Product { get; set; }
}
