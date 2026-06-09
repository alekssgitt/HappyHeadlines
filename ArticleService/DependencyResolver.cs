using ArticleService.Application.Interfaces;
using ArticleService.Infrastructure.Health;
using Common.Shared.Health;
using ArticleService.Application.Interfaces.Caching;
using ArticleService.Application.Interfaces.Data;
using ArticleService.Infrastructure.Caching;
using ArticleService.Infrastructure;
using ArticleService.Infrastructure.Queue;
using ArticleService.Infrastructure.Repositories;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace ArticleService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterDatabaseRouter(builder.Configuration);
        builder.Services.RegisterQueue(builder.Configuration);
        builder.Services.RegisterCache(builder.Configuration);
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
        builder.Services.AddHostedService<ArticleQueueConsumer>();
        builder.Services.AddHostedService<ArticleCachePreloadWorker>();
        builder.RegisterHealthChecks();
    }

    private static void RegisterHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddLivenessCheck()
            .AddCheck<ArticleDbReadinessCheck>("article_db", tags: ["ready"]);
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IArticleService, Application.Services.ArticleService>();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IArticleRepository, ArticleRepository>();
    }

    private static void RegisterCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration["Cache:ArticleRedis"] ?? "article-cache:6379"));
        services.AddSingleton<IArticleCacheService, ArticleCacheService>();
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

