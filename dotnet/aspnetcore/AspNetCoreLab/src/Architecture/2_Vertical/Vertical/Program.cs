using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Vertical.Database;
using Vertical.Features.Deposits.Registration;
using Vertical.Observability;
using Vertical.Pipeline;
using Vertical.Pipeline.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VerticalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VerticalDbContext")));

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    // Can be merged if necessary
    .AddOpenBehavior(typeof(LoggingBehavior<,>))
    .AddOpenBehavior(typeof(MetricsBehavior<,>))
    .AddOpenBehavior(typeof(TracingBehavior<,>))
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<Dispatcher>();

builder.Services.AddControllers();

// Features
builder.AddUsers();
builder.AddAuth();
builder.AddAccounts();
builder.AddDeposits();
builder.AddWithdrawals();

var app = builder.Build();

Telemetry.Init("Vertical");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapMetrics();

app.MapControllers();

app.Run();
