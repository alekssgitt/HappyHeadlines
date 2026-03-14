namespace PublisherService.Application.Interfaces.Queue;

public interface IArticleQueuePublisher
{
    Task PublishAsync(string payload, IDictionary<string, object?> headers);
}
