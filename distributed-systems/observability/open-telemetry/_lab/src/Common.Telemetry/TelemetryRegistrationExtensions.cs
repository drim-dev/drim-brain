using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;

namespace Common.Telemetry;

public static class TelemetryRegistrationExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IHostBuilder builder, string serviceName)
    {
        Tracing.Init(serviceName);

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName))
            .WithMetrics(metrics => metrics
                .AddMeter(serviceName)
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter())
            .WithTracing(tracing => tracing
                .AddSource(serviceName)
                .AddAspNetCoreInstrumentation()
                .AddGrpcClientInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter());

        builder.UseSerilog((context, configuration) =>
        {
            configuration
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Version", context.Configuration["Version"])
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new[] { new Uri("http://elasticsearch:9200") }, options =>
                {
                    options.DataStream = new DataStreamName("logs", "open-telemetry-lab", "drim");
                    options.BootstrapMethod = BootstrapMethod.Silent;
                });
        });

        return services;
    }

    public static WebApplication MapTelemetry(this WebApplication app)
    {
        app.MapPrometheusScrapingEndpoint();

        return app;
    }
}
