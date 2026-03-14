using NewsletterService.Application.Interfaces;
using NewsletterService.Infrastructure.Queue;
using RabbitMQ.Client;

namespace NewsletterService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterQueue(builder.Configuration);
        builder.Services.RegisterHttpClient(builder.Configuration);
        builder.Services.RegisterServices();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<LatestArticleBuffer>();
        services.AddScoped<INewsletterService, Application.Services.NewsletterService>();
        services.AddHostedService<ArticleQueueSubscriber>();
    }

    private static void RegisterHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("ArticleServiceClient", client =>
        {
            client.BaseAddress = new Uri(configuration["ArticleService:BaseUrl"] ?? "http://load-balancer");
            client.Timeout = TimeSpan.FromSeconds(10);
        });
    }

    private static void RegisterQueue(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["Queue:Host"] ?? "rabbitmq",
                Port = int.TryParse(configuration["Queue:Port"], out var port) ? port : 5672,
                UserName = configuration["Queue:UserName"] ?? "user",
                Password = configuration["Queue:Password"] ?? "pass",
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        });
    }
}
