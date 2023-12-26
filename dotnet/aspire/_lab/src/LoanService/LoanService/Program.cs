using AspireLab.ServiceDefaults;
using LoanService;
using LoanService.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<LoanServiceDbContext>("LoanServiceDb", settings =>
{
    // Can be set in configuration
    settings.DbContextPooling = false;
    settings.Metrics = true;
    settings.Tracing = true;
    settings.HealthChecks = true;
    settings.MaxRetryCount = 3;
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapAppEndpoints();

app.MigrateDb();

app.Run();
