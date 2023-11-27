using System.Reflection;
using ApiGateway.Clients;
using ApiGateway.Metrics;
using Common.Telemetry;
using Common.Validation;
using Common.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetry(builder.Host, builder.Environment.ApplicationName);

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddSingleton<ApiGatewayMetrics>();

var clientsOptions = builder.Configuration.GetSection(ClientsOptions.SectionName).Get<ClientsOptions>();

builder.Services.AddGrpcClient<BankingService.Client.Withdrawals.WithdrawalsClient>(o =>
{
    o.Address = new Uri(clientsOptions!.BankingService);
});

var app = builder.Build();

app.MapTelemetry();

app.MapEndpoints();

app.Run();
