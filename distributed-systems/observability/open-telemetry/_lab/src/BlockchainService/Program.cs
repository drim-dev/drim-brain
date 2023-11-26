using System.Reflection;
using BlockchainService.Features.Withdrawals.Registration;
using BlockchainService.Telemetry;
using Common.Validation;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

var tracingSourceName = builder.Environment.ApplicationName;
Tracing.Init(tracingSourceName);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(builder.Environment.ApplicationName))
    .WithMetrics(metrics => metrics
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddSource(tracingSourceName)
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter());

builder.Services.AddGrpc();

builder.AddWithdrawals();

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

app.MapWithdrawals();

app.Run();
