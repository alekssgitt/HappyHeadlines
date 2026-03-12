using Common.Shared.Monitoring;
using DraftService;
using DraftService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedMonitoring("DraftService");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterCustomServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>();
    var retries = 5;
    for (var attempt = 1; attempt <= retries; attempt++)
    {
        try
        {
            db.Database.EnsureCreated();
            app.Logger.LogInformation("DraftDatabase ready");
            break;
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "attempt {Attempt}/{Retries} failed to initialise DraftDatabase",
                attempt, retries);
            if (attempt == retries)
                app.Logger.LogError("giving up on DraftDatabase after {Retries} attempts", retries);
            else
                Thread.Sleep(2000);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
