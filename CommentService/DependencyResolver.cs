using CommentService.Application.Interfaces;
using CommentService.Application.Interfaces.Caching;
using CommentService.Application.Interfaces.Data;
using CommentService.Application.Interfaces.External;
using CommentService.Infrastructure;
using CommentService.Infrastructure.Caching;
using CommentService.Infrastructure.External;
using CommentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;

namespace CommentService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.RegisterDbContext();
        builder.Services.RegisterCache(builder.Configuration);
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
        builder.RegisterProfanityClient();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ICommentService, Application.Services.CommentService>();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICommentRepository, CommentRepository>();
    }

    private static void RegisterCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration["Cache:CommentRedis"] ?? "comment-cache:6379"));
        services.AddSingleton<ICommentCacheService, CommentCacheService>();
    }

    private static void RegisterDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<CommentDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("CommentDb")));
    }

    private static void RegisterProfanityClient(this WebApplicationBuilder builder)
    {
        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, timespan) =>
                {
                    Console.WriteLine($"[CircuitBreaker] OPEN — ProfanityService unavailable. " +
                                      $"Breaking for {timespan.TotalSeconds}s. " +
                                      $"Reason: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                },
                onReset: () => Console.WriteLine("[CircuitBreaker] CLOSED — ProfanityService recovered"),
                onHalfOpen: () => Console.WriteLine("[CircuitBreaker] HALF-OPEN — testing ProfanityService")
            );
        
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(1, _ => TimeSpan.FromMilliseconds(500));
        
        builder.Services
            .AddHttpClient<ProfanityClient>(client =>
            {
                var baseUrl = builder.Configuration["ProfanityService:BaseUrl"]
                              ?? "http://profanity-service:8080";
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

       
        builder.Services.AddScoped<IProfanityClient>(sp =>
        {
            var inner = sp.GetRequiredService<ProfanityClient>();
            var logger = sp.GetRequiredService<ILogger<ProfanityClientFallback>>();
            return new ProfanityClientFallback(inner, logger);
        });
    }
}
