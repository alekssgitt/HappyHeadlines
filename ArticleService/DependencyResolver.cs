using ArticleService.Application.Interfaces;
using ArticleService.Application.Interfaces.Data;
using ArticleService.Infrastructure;
using ArticleService.Infrastructure.Queue;
using ArticleService.Infrastructure.Repositories;
using RabbitMQ.Client;

namespace ArticleService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterDatabaseRouter(builder.Configuration);
        builder.Services.RegisterQueue(builder.Configuration);
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
        builder.Services.AddHostedService<ArticleQueueConsumer>();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IArticleService, Application.Services.ArticleService>();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IArticleRepository, ArticleRepository>();
    }

    private static void RegisterDatabaseRouter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseRouter>(sp =>
            new DatabaseRouter(configuration));
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

