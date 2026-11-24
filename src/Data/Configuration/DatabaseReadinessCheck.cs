using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LedgerAI.Api.Data.Context;

namespace LedgerAI.Api.Data.Configuration;

public static class DatabaseReadinessCheck
{
    public static async Task CheckAsync(
        LedgerDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await context.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                logger.LogError("Database readiness check failed: PostgreSQL is unreachable");
                return;
            }

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            var pendingCount = pendingMigrations.Count();
            if (pendingCount > 0)
            {
                logger.LogWarning(
                    "Database readiness check found {PendingMigrationCount} pending EF Core migration(s)",
                    pendingCount);
            }
            else
            {
                logger.LogInformation("Database readiness check passed; no pending EF Core migrations");
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Database readiness check failed");
        }
    }
}
