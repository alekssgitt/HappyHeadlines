using SubscriberService.Application.Interfaces.FeatureFlags;

namespace SubscriberService.Infrastructure.FeatureFlags;

public class ReleaseToggleService(IConfiguration configuration) : IReleaseToggleService
{
    private volatile bool _subscriberServiceEnabled =
        bool.TryParse(configuration["ReleaseToggles:SubscriberServiceEnabled"], out var enabled) && enabled;

    public bool IsSubscriberServiceEnabled() => _subscriberServiceEnabled;

    public bool SetSubscriberServiceEnabled(bool enabled)
    {
        _subscriberServiceEnabled = enabled;
        return _subscriberServiceEnabled;
    }
}
