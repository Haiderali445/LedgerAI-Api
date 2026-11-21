using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Modules.Catalog;
using LedgerAI.Api.Modules.Inventory;
using LedgerAI.Api.Modules.Orders;

namespace LedgerAI.Api.Modules.Invoicing;

public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid? OrderItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid BatchId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ConversionFactor { get; set; } = 1m;
    public decimal BaseQuantity { get; set; }
    public decimal BaseUnitCost { get; set; }
    public decimal LineTotal { get; set; }
    public decimal LineCogs { get; set; }

    // Navigation properties
    public Invoice? Invoice { get; set; }
    public OrderItem? OrderItem { get; set; }
    public Warehouse? Warehouse { get; set; }
    public Product? Product { get; set; }
    public StockBatch? Batch { get; set; }
    public UnitOfMeasure? Unit { get; set; }
}
