using AccountService;
using AspireLab.ServiceDefaults;
using BlockchainService.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<BlockchainServiceClient>(
    static client=> client.BaseAddress = new("http://blockchain-service"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapAppEndpoints();

app.Run();
