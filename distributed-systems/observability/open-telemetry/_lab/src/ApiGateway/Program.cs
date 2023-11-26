using ApiGateway.Clients;
using ApiGateway.Metrics;
using Common.Web.Endpoints;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(builder.Environment.ApplicationName))
    .WithMetrics(metrics => metrics
        .AddMeter(ApiGatewayMetrics.MeterName)
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter());

builder.Services.AddSingleton<ApiGatewayMetrics>();

var clientsOptions = builder.Configuration.GetSection(ClientsOptions.SectionName).Get<ClientsOptions>();

builder.Services.AddGrpcClient<BankingService.Client.Withdrawals.WithdrawalsClient>(o =>
{
    o.Address = new Uri(clientsOptions!.BankingService);
});

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

app.MapEndpoints();

app.Run();
