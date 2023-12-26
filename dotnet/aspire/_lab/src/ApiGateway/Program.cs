using AccountService.Api;
using ApiGateway;
using AspireLab.ServiceDefaults;
using LoanService.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisOutputCache("redis", static settings =>
{
    // Can be set in configuration
    settings.Tracing = true;
    settings.HealthChecks = true;
});

builder.Services.AddHttpClient<LoanServiceClient>(
    static client=> client.BaseAddress = new("http://loan-service"));

builder.Services.AddHttpClient<AccountServiceClient>(
    static client=> client.BaseAddress = new("http://account-service"));

var app = builder.Build();

app.UseOutputCache();

app.MapDefaultEndpoints();

app.MapAppEndpoints();

app.Run();
