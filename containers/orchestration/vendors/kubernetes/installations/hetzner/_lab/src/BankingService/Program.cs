using System.Reflection;
using BankingService.Clients;
using BankingService.Database;
using BankingService.Features.Withdrawals.Registration;
using Common.Validation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BankingDbContext")));

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

var configuration = builder.Configuration
    .AddJsonFile(Path.Combine("extraSettings", "appsettings.json"), optional: false, reloadOnChange: true)
    .Build();

var clientsOptions = configuration.GetSection(ClientsOptions.SectionName).Get<ClientsOptions>();
Console.WriteLine($"BlockchainService address: {clientsOptions!.BlockchainService}");

builder.Services.AddGrpcClient<BlockchainService.Client.Withdrawals.WithdrawalsClient>(o =>
{
    o.Address = new Uri(clientsOptions!.BlockchainService);
});

builder.Services.AddGrpc();

builder.AddWithdrawals();

var app = builder.Build();

app.MapWithdrawals();

var command = args.FirstOrDefault();
if (command == "migrate")
{
    await app.MigrateDatabase();
}
else
{
    app.Run();
}
