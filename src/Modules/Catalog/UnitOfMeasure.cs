using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Catalog;

public class UnitOfMeasure : BaseEntity
{
    public string UnitCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool AllowFractional { get; set; } = false;
    public string AliasesJson { get; set; } = "[]";
    public bool IsActive { get; set; } = true;
}
