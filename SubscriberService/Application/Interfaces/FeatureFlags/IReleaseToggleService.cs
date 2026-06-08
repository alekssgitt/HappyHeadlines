namespace SubscriberService.Application.Interfaces.FeatureFlags;

public interface IReleaseToggleService
{
    bool IsSubscriberServiceEnabled();
    bool SetSubscriberServiceEnabled(bool enabled);
}
