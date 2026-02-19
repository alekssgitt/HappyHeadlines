using ArticleService;
using ArticleService.Application.Interfaces.Data;

var builder = WebApplication.CreateBuilder(args);

// Add framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register all custom services (router, repos, services)
builder.RegisterCustomServices();

var app = builder.Build();

// Auto-create tables on every database shard at startup (z-axis split)
using (var scope = app.Services.CreateScope())
{
    var router = scope.ServiceProvider.GetRequiredService<IDatabaseRouter>();

    foreach (var continent in router.ValidContinents)
    {
        var retries = 5;
        for (var attempt = 1; attempt <= retries; attempt++)
        {
            try
            {
                using var db = router.GetDbContext(continent);
                db.Database.EnsureCreated();
                app.Logger.LogInformation("Database shard ready: {Continent}", continent);
                break;
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex,
                    "Attempt {Attempt}/{Retries} — failed to initialise shard '{Continent}'",
                    attempt, retries, continent);

                if (attempt == retries)
                    app.Logger.LogError("Giving up on shard '{Continent}' after {Retries} attempts",
                        continent, retries);
                else
                    Thread.Sleep(2000);
            }
        }
    }
}

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
