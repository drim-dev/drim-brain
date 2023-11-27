using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;

namespace Common.Logging;

public static class SerilogRegistrationExtensions
{
    public static IHostBuilder SetupSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, configuration) =>
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
        });;
    }
}
