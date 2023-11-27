using System.Reflection;
using BankingService.Clients;
using BankingService.Database;
using BankingService.Features.Withdrawals.Registration;
using Common.Telemetry;
using Common.Validation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetry(builder.Host, builder.Environment.ApplicationName);

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BankingDbContext")));

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

await app.MigrateDatabase();

app.Run();
