using System.Reflection;
using Clean.Database;
using Clean.Observability;
using Clean.WebApplicationBuilderExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CleanDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CleanDbContext")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.AddHostedServices();
builder.AddRepositories();
builder.AddServices();
builder.AddOptions();

builder.Services.AddControllers();

var app = builder.Build();

Telemetry.Init("Clean");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapMetrics();

app.MapControllers();

app.Run();
