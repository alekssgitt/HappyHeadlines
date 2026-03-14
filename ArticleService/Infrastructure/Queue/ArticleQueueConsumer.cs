using ArticleService.Application.Interfaces.Data;
using ArticleService.Domain;
using Common.Shared.Monitoring;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ArticleService.Infrastructure.Queue;

public class ArticleQueueConsumer(
    IConnection connection,
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<ArticleQueueConsumer> logger) : BackgroundService
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
                "ArticleService Consume ArticleQueue",
                ActivityKind.Consumer,
                parentContext);

            try
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var article = JsonSerializer.Deserialize<Article>(json);

                if (article is not null)
                {
                    using var scope = scopeFactory.CreateScope();
                    var dbRouter = scope.ServiceProvider.GetRequiredService<IDatabaseRouter>();

                    using var db = dbRouter.GetDbContext(article.Continent);
                    var exists = await db.Articles.AsNoTracking().AnyAsync(a => a.Id == article.Id, stoppingToken);
                    if (!exists)
                    {
                        db.Articles.Add(article);
                        await db.SaveChangesAsync(stoppingToken);
                        logger.LogInformation("Persisted queued article {ArticleId} in shard {Continent}",
                            article.Id, article.Continent);
                    }
                    else
                    {
                        logger.LogInformation("Skipped duplicate queued article {ArticleId}", article.Id);
                    }
                }

                channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process queue message in ArticleService");
                channel.BasicNack(eventArgs.DeliveryTag, false, true);
            }

            await Task.CompletedTask;
        };

        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
