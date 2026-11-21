using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Catalog;

public class Product : BaseEntity
{
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public Guid BaseUnitId { get; set; }
    public Guid? SecondaryUnitId { get; set; }
    public decimal PackSize { get; set; } = 1m;
    public string? Barcode { get; set; }
    public decimal DefaultPurchasePrice { get; set; }
    public decimal WeightedAvgCost { get; set; }
    public decimal DefaultSalesPrice { get; set; }
    public decimal MinimumStock { get; set; }
    public bool IsService { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public string AttributesJson { get; set; } = "{\"aliases\": []}";
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public Category? Category { get; set; }
    public UnitOfMeasure? BaseUnit { get; set; }
    public UnitOfMeasure? SecondaryUnit { get; set; }
}
