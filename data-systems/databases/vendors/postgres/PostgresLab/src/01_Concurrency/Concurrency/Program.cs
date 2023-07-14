using System.Reflection;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Transactions.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("Transactions")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
