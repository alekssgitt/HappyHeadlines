using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Shared.Health;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddLivenessCheck(this IHealthChecksBuilder builder) =>
        builder.AddCheck(
            "liveness",
            () => HealthCheckResult.Healthy("Service process is running"),
            tags: ["live"]);

    public static IHealthChecksBuilder AddBasicReadyCheck(this IHealthChecksBuilder builder) =>
        builder.AddCheck(
            "liveness",
            () => HealthCheckResult.Healthy("Service process is running"),
            tags: ["live", "ready"]);

    public static WebApplication MapServiceHealthEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("live")
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready"),
            ResultStatusCodes =
            {
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable
            }
        });

        app.MapHealthChecks("/health");
        return app;
    }
}
