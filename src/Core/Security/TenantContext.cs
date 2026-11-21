using LedgerAI.Api.Core.Interfaces;

namespace LedgerAI.Api.Core.Security;

public class TenantContext : ITenantProvider
{
    private Guid? _tenantId;
    private string? _tenantCode;

    public Guid? GetCurrentTenantId() => _tenantId;
    public string? GetCurrentTenantCode() => _tenantCode;

    public void SetTenant(Guid tenantId, string? tenantCode = null)
    {
        _tenantId = tenantId;
        _tenantCode = tenantCode;
    }
}
