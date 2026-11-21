namespace LedgerAI.Api.Core.Interfaces;

public interface ITenantProvider
{
    Guid? GetCurrentTenantId();
    string? GetCurrentTenantCode();
    void SetTenant(Guid tenantId, string? tenantCode = null);
}
