using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Common.Shared.Monitoring;

public static class MonitoringExtensions
{
    public static WebApplicationBuilder AddSharedMonitoring(this WebApplicationBuilder builder, string serviceName)
    {
        var seqServerUrl = builder.Configuration["Monitoring:Seq:ServerUrl"] ?? "http://seq:5341";
        var zipkinEndpoint = builder.Configuration["Monitoring:Zipkin:Endpoint"] ?? "http://zipkin:9411/api/v2/spans";

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .WriteTo.Console()
                .WriteTo.Seq(seqServerUrl);
        });

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource("HappyHeadlines.Messaging")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddZipkinExporter(options => { options.Endpoint = new Uri(zipkinEndpoint); });
            });

        return builder;
    }
}
