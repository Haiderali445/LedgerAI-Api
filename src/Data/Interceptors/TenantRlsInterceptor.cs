using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using LedgerAI.Api.Core.Interfaces;

namespace LedgerAI.Api.Data.Interceptors;

public class TenantRlsInterceptor : DbConnectionInterceptor
{
    private readonly ITenantProvider _tenantProvider;

    public TenantRlsInterceptor(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (tenantId.HasValue && tenantId.Value != Guid.Empty)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"SET LOCAL app.current_tenant = '{tenantId.Value}';";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}
