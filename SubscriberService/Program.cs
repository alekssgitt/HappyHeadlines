using Common.Shared.Health;
using Common.Shared.Monitoring;
using SubscriberService;
using SubscriberService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedMonitoring("SubscriberService");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterCustomServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SubscriberDbContext>();
    var retries = 5;
    for (var attempt = 1; attempt <= retries; attempt++)
    {
        try
        {
            db.Database.EnsureCreated();
            app.Logger.LogInformation("SubscriberDatabase ready");
            break;
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "attempt {Attempt}/{Retries} failed to initialise SubscriberDatabase",
                attempt, retries);
            if (attempt == retries)
                app.Logger.LogError("giving up on SubscriberDatabase after {Retries} attempts", retries);
            else
                Thread.Sleep(2000);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapServiceHealthEndpoints();

app.Run();
