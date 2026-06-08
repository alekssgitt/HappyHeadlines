using RabbitMQ.Client;
using SubscriberService.Application.Interfaces.Queue;
using SubscriberService.Domain;
using System.Text;
using System.Text.Json;

namespace SubscriberService.Infrastructure.Queue;

public class SubscriberQueuePublisher(IConnection connection, IConfiguration configuration) : ISubscriberQueuePublisher
{
    private readonly string _queueName = configuration["Queue:SubscriberQueueName"] ?? "subscriber-queue";

    public Task PublishNewSubscriberAsync(Subscriber subscriber, IDictionary<string, object?> headers)
    {
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Headers = headers
            .Where(h => h.Value is not null)
            .ToDictionary(h => h.Key, h => h.Value!);

        var payload = JsonSerializer.Serialize(new
        {
            subscriber.Id,
            subscriber.Email,
            subscriber.CreatedAt
        });

        var body = Encoding.UTF8.GetBytes(payload);
        channel.BasicPublish(string.Empty, _queueName, properties, body);
        return Task.CompletedTask;
    }
}
