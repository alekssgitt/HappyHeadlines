using SubscriberService.Domain;

namespace SubscriberService.Application.Interfaces.Queue;

public interface ISubscriberQueuePublisher
{
    Task PublishNewSubscriberAsync(Subscriber subscriber, IDictionary<string, object?> headers);
}
