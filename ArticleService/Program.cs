using ArticleService;
using ArticleService.Application.Interfaces.Data;
using Common.Shared.Monitoring;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedMonitoring("ArticleService");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterCustomServices();

var app = builder.Build();

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
                app.Logger.LogInformation("db shard ready: {Continent}", continent);
                break;
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex,
                    "attempt {Attempt}/{Retries} failed to initialise shard '{Continent}'",
                    attempt, retries, continent);

                if (attempt == retries)
                    app.Logger.LogError("giving up on db shard '{Continent}' after {Retries} attempts",
                        continent, retries);
                else
                    Thread.Sleep(2000);
            }
        }
    }
}


app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

app.Run();
