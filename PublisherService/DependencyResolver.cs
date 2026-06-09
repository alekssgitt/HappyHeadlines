using Common.Shared.Health;
using PublisherService.Application.Interfaces;
using PublisherService.Application.Interfaces.Queue;
using PublisherService.Infrastructure.Queue;
using RabbitMQ.Client;

namespace PublisherService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterQueue(builder.Configuration);
        builder.Services.RegisterServices();
        builder.RegisterHealthChecks();
    }

    private static void RegisterHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddBasicReadyCheck();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IPublisherService, Application.Services.PublisherService>();
        services.AddScoped<IArticleQueuePublisher, ArticleQueuePublisher>();
    }

    private static void RegisterQueue(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["Queue:Host"] ?? "rabbitmq",
                Port = int.TryParse(configuration["Queue:Port"], out var port) ? port : 5672,
                UserName = configuration["Queue:UserName"] ?? "guest",
                Password = configuration["Queue:Password"] ?? "guest",
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        });
    }
}
