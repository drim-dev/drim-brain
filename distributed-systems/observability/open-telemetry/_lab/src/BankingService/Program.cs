using System.Reflection;
using BankingService.Clients;
using BankingService.Features.Withdrawals.Registration;
using Common.Telemetry;
using Common.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetry(builder.Host, builder.Environment.ApplicationName);

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

var clientsOptions = builder.Configuration.GetSection(ClientsOptions.SectionName).Get<ClientsOptions>();

builder.Services.AddGrpcClient<BlockchainService.Client.Withdrawals.WithdrawalsClient>(o =>
{
    o.Address = new Uri(clientsOptions!.BlockchainService);
});

builder.Services.AddGrpc();

builder.AddWithdrawals();

var app = builder.Build();

app.MapTelemetry();

app.MapWithdrawals();

app.Run();
