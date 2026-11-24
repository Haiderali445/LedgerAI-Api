using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;
using LedgerAI.Api.Core.Interfaces;

namespace LedgerAI.Api.Data.Interceptors;

public class TenantRlsInterceptor : DbConnectionInterceptor
{
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<TenantRlsInterceptor> _logger;

    public TenantRlsInterceptor(ITenantProvider tenantProvider, ILogger<TenantRlsInterceptor> logger)
    {
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    private static string GetTarget(DbConnection connection)
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connection.ConnectionString);
            return $"{builder.Host}:{builder.Port}/{builder.Database}";
        }
        catch
        {
            return "configured-postgres";
        }
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (tenantId.HasValue && tenantId.Value != Guid.Empty)
            SetTenantContext(connection, tenantId.Value);
        else
            _logger.LogWarning("Database connection opened without an active tenant context");

        _logger.LogInformation("Database connection opened to {DatabaseTarget}", GetTarget(connection));
        base.ConnectionOpened(connection, eventData);
    }

    public override void ConnectionClosed(DbConnection connection, ConnectionEndEventData eventData)
    {
        _logger.LogInformation("Database connection closed for {DatabaseTarget}", GetTarget(connection));
        base.ConnectionClosed(connection, eventData);
    }

    public override void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
    {
        _logger.LogError(eventData.Exception, "Database connection failed for {DatabaseTarget}", GetTarget(connection));
        base.ConnectionFailed(connection, eventData);
    }

    public override async Task ConnectionFailedAsync(
        DbConnection connection,
        ConnectionErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _logger.LogError(eventData.Exception, "Database connection failed for {DatabaseTarget}", GetTarget(connection));
        await base.ConnectionFailedAsync(connection, eventData, cancellationToken);
    }

    private void SetTenantContext(DbConnection connection, Guid tenantId)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT set_config('app.current_tenant', @tenantId, false)";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@tenantId";
        parameter.Value = tenantId.ToString();
        command.Parameters.Add(parameter);
        command.ExecuteNonQuery();
        _logger.LogDebug("Set app.current_tenant for database session");
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
            command.CommandText = "SELECT set_config('app.current_tenant', @tenantId, false)";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@tenantId";
            parameter.Value = tenantId.Value.ToString();
            command.Parameters.Add(parameter);
            await command.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogDebug("Set app.current_tenant for database session");
        }
        else
        {
            _logger.LogWarning("Database connection opened without an active tenant context");
        }

        _logger.LogInformation("Database connection opened to {DatabaseTarget}", GetTarget(connection));
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}
