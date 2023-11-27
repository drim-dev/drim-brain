using System.Reflection;
using BlockchainService.Features.Withdrawals.Registration;
using Common.Telemetry;
using Common.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetry(builder.Host, builder.Environment.ApplicationName);

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddGrpc();

builder.AddWithdrawals();

var app = builder.Build();

app.MapTelemetry();

app.MapWithdrawals();

app.Run();
