using AccountService.Api;
using ApiGateway;
using AspireLab.ServiceDefaults;
using LoanService.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<LoanServiceClient>(
    static client=> client.BaseAddress = new("http://loan-service"));

builder.Services.AddHttpClient<AccountServiceClient>(
    static client=> client.BaseAddress = new("http://account-service"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapAppEndpoints();

app.Run();
