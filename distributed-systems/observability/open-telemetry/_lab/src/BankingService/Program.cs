using System.Reflection;
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

builder.Services.AddGrpc();

builder.AddWithdrawals();

var app = builder.Build();

app.MapWithdrawals();

app.Run();
