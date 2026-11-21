using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.TenantAdmin;

public interface ITenantService
{
    Task<Result<Tenant>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Tenant>> GetByCodeAsync(string tenantCode, CancellationToken cancellationToken = default);
    Task<Result<Tenant>> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
