using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.TenantAdmin;

public class User : BaseEntity
{
    public string UserCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string Role { get; set; } = "Operator";
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Tenant? Tenant { get; set; }
}
