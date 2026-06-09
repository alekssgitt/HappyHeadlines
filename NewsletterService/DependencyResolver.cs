using Common.Shared.Health;
using NewsletterService.Application.Interfaces;
using NewsletterService.Infrastructure.External;
using NewsletterService.Infrastructure.Queue;
using Polly;
using Polly.Extensions.Http;
using RabbitMQ.Client;

namespace NewsletterService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterQueue(builder.Configuration);
        builder.Services.RegisterHttpClients(builder.Configuration);
        builder.Services.RegisterServices();
        builder.RegisterHealthChecks();
    }

    private static void RegisterHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddBasicReadyCheck();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<LatestArticleBuffer>();
        services.AddScoped<INewsletterService, Application.Services.NewsletterService>();
        services.AddScoped<ISubscriberClient>(sp =>
        {
            var inner = sp.GetRequiredService<SubscriberClient>();
            var logger = sp.GetRequiredService<ILogger<SubscriberClientFallback>>();
            return new SubscriberClientFallback(inner, logger);
        });

        services.AddHostedService<ArticleQueueSubscriber>();
        services.AddHostedService<SubscriberQueueSubscriber>();
    }

    private static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("ArticleServiceClient", client =>
        {
            client.BaseAddress = new Uri(configuration["ArticleService:BaseUrl"] ?? "http://load-balancer");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(1, _ => TimeSpan.FromMilliseconds(500));

        services
            .AddHttpClient<SubscriberClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["SubscriberService:BaseUrl"] ?? "http://subscriber-service");
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);
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
