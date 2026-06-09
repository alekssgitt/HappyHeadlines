using Common.Shared.Health;
using Common.Shared.Monitoring;
using PublisherService;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedMonitoring("PublisherService");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterCustomServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapServiceHealthEndpoints();

app.Run();
