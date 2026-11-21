using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.TenantAdmin;

public class TenantService : ITenantService
{
    public Task<Result<Tenant>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Tenant>("Not implemented."));
    }

    public Task<Result<Tenant>> GetByCodeAsync(string tenantCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Tenant>("Not implemented."));
    }

    public Task<Result<Tenant>> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Tenant>("Not implemented."));
    }
}
