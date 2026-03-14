using CommentService;
using CommentService.Infrastructure;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterCustomServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    var retries = 5;
    for (var attempt = 1; attempt <= retries; attempt++)
    {
        try
        {
            db.Database.EnsureCreated();
            app.Logger.LogInformation("CommentDatabase ready");
            break;
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "attempt {Attempt}/{Retries} failed to initialise CommentDatabase",
                attempt, retries);
            if (attempt == retries)
                app.Logger.LogError("giving up on CommentDatabase after {Retries} attempts", retries);
            else
                Thread.Sleep(2000);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

app.Run();
