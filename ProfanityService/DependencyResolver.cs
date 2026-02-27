using Microsoft.EntityFrameworkCore;
using ProfanityService.Application.Interfaces;
using ProfanityService.Application.Interfaces.Data;
using ProfanityService.Infrastructure;
using ProfanityService.Infrastructure.Repositories;

namespace ProfanityService;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.RegisterDbContext();
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IProfanityService, Application.Services.ProfanityService>();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProfanityRepository, ProfanityRepository>();
    }

    private static void RegisterDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ProfanityDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("ProfanityDb")));
    }
}
