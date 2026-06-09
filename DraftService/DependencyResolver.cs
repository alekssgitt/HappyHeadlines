using Common.Shared.Health;
using DraftService.Application.Interfaces;
using DraftService.Application.Interfaces.Data;
using DraftService.Infrastructure;
using DraftService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DraftService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.RegisterDbContext();
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
        builder.RegisterHealthChecks();
    }

    private static void RegisterHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddLivenessCheck()
            .AddDbContextCheck<DraftDbContext>(tags: ["ready"]);
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IDraftService, Application.Services.DraftService>();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDraftRepository, DraftRepository>();
    }

    private static void RegisterDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DraftDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DraftDb")));
    }
}
