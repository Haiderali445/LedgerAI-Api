using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Modules.Catalog;

namespace LedgerAI.Api.Modules.Inventory;

public class StockReservation : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid BatchId { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal ReleasedQuantity { get; set; }
    public decimal CommittedQuantity { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public Product? Product { get; set; }
    public StockBatch? Batch { get; set; }
}
