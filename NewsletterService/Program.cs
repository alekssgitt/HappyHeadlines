using Common.Shared.Monitoring;
using NewsletterService;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedMonitoring("NewsletterService");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterCustomServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
