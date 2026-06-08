using Common.Shared.Monitoring;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace NewsletterService.Infrastructure.Queue;

public class SubscriberQueueSubscriber(
    IConnection connection,
    IConfiguration configuration,
    ILogger<SubscriberQueueSubscriber> logger) : BackgroundService
{
    private readonly string _queueName = configuration["Queue:SubscriberQueueName"] ?? "subscriber-queue";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
        channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, eventArgs) =>
        {
            var parentContext = MessagingTraceContext.ExtractActivityContext(eventArgs.BasicProperties.Headers);
            using var activity = MessagingTraceContext.ActivitySource.StartActivity(
                "NewsletterService Consume SubscriberQueue",
                ActivityKind.Consumer,
                parentContext);

            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var payload = JsonSerializer.Deserialize<NewSubscriberPayload>(json);

            if (payload is not null)
            {
                logger.LogInformation("Welcome mail triggered for new subscriber {Email}", payload.Email);
            }

            channel.BasicAck(eventArgs.DeliveryTag, false);
            await Task.CompletedTask;
        };

        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private class NewSubscriberPayload
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
