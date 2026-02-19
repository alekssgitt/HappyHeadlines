using ArticleService.Application.Interfaces;
using ArticleService.Application.Interfaces.Data;
using ArticleService.Infrastructure;
using ArticleService.Infrastructure.Repositories;

namespace ArticleService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterDatabaseRouter(builder.Configuration);
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
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
}

