using Common.Shared.Monitoring;
using NewsletterService.Application.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace NewsletterService.Infrastructure.Queue;

public class ArticleQueueSubscriber(
    IConnection connection,
    IConfiguration configuration,
    LatestArticleBuffer buffer,
    ILogger<ArticleQueueSubscriber> logger) : BackgroundService
{
    private readonly string _queueName = configuration["Queue:ArticleQueueName"] ?? "article-queue";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = connection.CreateModel();
        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, eventArgs) =>
        {
            var parentContext = MessagingTraceContext.ExtractActivityContext(eventArgs.BasicProperties.Headers);
            using var activity = MessagingTraceContext.ActivitySource.StartActivity(
                "NewsletterService Consume ArticleQueue",
                ActivityKind.Consumer,
                parentContext);

            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var article = JsonSerializer.Deserialize<ArticleSummaryDto>(json);

            if (article is not null)
            {
                buffer.Add(article);
                logger.LogInformation("Queued article {ArticleId} captured for newsletter", article.Id);
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
}
