using SubscriberService.Application.Interfaces.FeatureFlags;

namespace SubscriberService.Infrastructure.FeatureFlags;

public class ReleaseToggleService(
    IConfiguration configuration,
    ILogger<ReleaseToggleService> logger) : IReleaseToggleService
{
    private volatile bool _subscriberServiceEnabled =
        bool.TryParse(configuration["ReleaseToggles:SubscriberServiceEnabled"], out var enabled) && enabled;

    public bool IsSubscriberServiceEnabled() => _subscriberServiceEnabled;

    public bool SetSubscriberServiceEnabled(bool enabled)
    {
        _subscriberServiceEnabled = enabled;
        if (enabled)
            logger.LogInformation("Release toggle ON — SubscriberService ready for traffic (/health/ready will pass)");
        else
            logger.LogWarning(
                "Rollback: release toggle OFF — SubscriberService not ready (/health/ready fails, subscribe returns 503)");

        return _subscriberServiceEnabled;
    }
}
