using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Inventory;

public class Warehouse : BaseEntity
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
