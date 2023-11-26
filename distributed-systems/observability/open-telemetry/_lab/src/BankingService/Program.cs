using System.Reflection;
using BankingService.Clients;
using BankingService.Database;
using BankingService.Features.Withdrawals.Registration;
using Common.Validation;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BankingDbContext")));

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(builder.Environment.ApplicationName))
    .WithMetrics(metrics => metrics
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter());

var clientsOptions = builder.Configuration.GetSection(ClientsOptions.SectionName).Get<ClientsOptions>();

builder.Services.AddGrpcClient<BlockchainService.Client.Withdrawals.WithdrawalsClient>(o =>
{
    o.Address = new Uri(clientsOptions!.BlockchainService);
});

builder.Services.AddGrpc();

builder.AddWithdrawals();

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

app.MapWithdrawals();

app.Run();
