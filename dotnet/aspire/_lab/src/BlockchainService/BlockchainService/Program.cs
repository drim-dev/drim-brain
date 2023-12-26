using AspireLab.ServiceDefaults;
using BlockchainService;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapAppEndpoints();

app.Run();
