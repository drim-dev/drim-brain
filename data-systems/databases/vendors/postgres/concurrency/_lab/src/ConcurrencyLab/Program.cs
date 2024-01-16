using System.Reflection;
using Concurrency.Database;
using Concurrency.Features.Withdrawals.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("Concurrency")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddFastEndpoints();

builder.AddWithdrawals();

var app = builder.Build();

app.UseFastEndpoints();

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();

app.Run();
