namespace LedgerAI.Api.Data.Dtos.Catalog;

public class UnitOfMeasureDto
{
    public Guid Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool AllowFractional { get; set; }
}
