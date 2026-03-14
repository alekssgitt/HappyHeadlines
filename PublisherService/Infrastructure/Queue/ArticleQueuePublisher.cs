using PublisherService.Application.Interfaces.Queue;
using RabbitMQ.Client;
using System.Text;

namespace PublisherService.Infrastructure.Queue;

public class ArticleQueuePublisher(IConnection connection, IConfiguration configuration) : IArticleQueuePublisher
{
    private readonly string _queueName = configuration["Queue:ArticleQueueName"] ?? "article-queue";

    public Task PublishAsync(string payload, IDictionary<string, object?> headers)
    {
        using var channel = connection.CreateModel();
        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Headers = headers
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value!);

        var body = Encoding.UTF8.GetBytes(payload);
        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _queueName,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}
