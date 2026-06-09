using Microsoft.Extensions.Diagnostics.HealthChecks;
using SubscriberService.Application.Interfaces.FeatureFlags;

namespace SubscriberService.Infrastructure.Health;

public class ReleaseToggleReadinessCheck(IReleaseToggleService releaseToggleService) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (releaseToggleService.IsSubscriberServiceEnabled())
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("SubscriberService release toggle is ON — accepting traffic"));
        }

        return Task.FromResult(
            HealthCheckResult.Unhealthy(
                "Rollback active: release toggle is OFF — service not ready for traffic"));
    }
}
