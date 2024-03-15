using System.Reflection;
using ApiGateway.Clients;
using ApiGateway.Metrics;
using Common.Validation;
using Common.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddSingleton<ApiGatewayMetrics>();

var configuration = builder.Configuration
    .AddJsonFile(Path.Combine("extraSettings", "appsettings.json"), optional: false, reloadOnChange: true)
    .Build();

var clientsOptions = configuration.GetSection(ClientsOptions.SectionName).Get<ClientsOptions>();
Console.WriteLine($"BankingService address: {clientsOptions!.BankingService}");

builder.Services.AddGrpcClient<BankingService.Client.Withdrawals.WithdrawalsClient>(o =>
{
    o.Address = new Uri(clientsOptions!.BankingService);
});

var app = builder.Build();

app.MapEndpoints();

app.Run();
