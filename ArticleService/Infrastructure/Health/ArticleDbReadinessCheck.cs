using ArticleService.Application.Interfaces.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ArticleService.Infrastructure.Health;

public class ArticleDbReadinessCheck(IDatabaseRouter databaseRouter) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var db = databaseRouter.GetDbContext("global");
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Global article shard is reachable")
                : HealthCheckResult.Unhealthy("Cannot connect to global article shard");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Article database check failed", ex);
        }
    }
}
