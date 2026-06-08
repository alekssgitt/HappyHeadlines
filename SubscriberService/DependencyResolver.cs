using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using SubscriberService.Application.Interfaces;
using SubscriberService.Application.Interfaces.Data;
using SubscriberService.Application.Interfaces.FeatureFlags;
using SubscriberService.Application.Interfaces.Queue;
using SubscriberService.Infrastructure;
using SubscriberService.Infrastructure.FeatureFlags;
using SubscriberService.Infrastructure.Queue;
using SubscriberService.Infrastructure.Repositories;

namespace SubscriberService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.RegisterDbContext();
        builder.Services.RegisterQueue(builder.Configuration);
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
    }

    private static void RegisterDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<SubscriberDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("SubscriberDb")));
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISubscriberRepository, SubscriberRepository>();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ISubscriberService, Application.Services.SubscriberService>();
        services.AddScoped<ISubscriberQueuePublisher, SubscriberQueuePublisher>();
        services.AddSingleton<IReleaseToggleService, ReleaseToggleService>();
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
