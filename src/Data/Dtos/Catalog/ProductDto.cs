namespace LedgerAI.Api.Data.Dtos.Catalog;

public class ProductDto
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid BaseUnitId { get; set; }
    public string? BaseUnitCode { get; set; }
    public decimal DefaultSalesPrice { get; set; }
    public decimal DefaultPurchasePrice { get; set; }
    public decimal MinimumStock { get; set; }
    public bool IsActive { get; set; }
}
