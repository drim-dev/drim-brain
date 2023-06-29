using System.Net;
using System.Reflection;
using FastEndpoints;
using NBitcoin;
using NBitcoin.RPC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddFastEndpoints();

builder.Services.AddSingleton(_ =>
{
    var credentials = new NetworkCredential("user", "password");
    var bitcoindUri = new Uri("http://127.0.0.1:18332");

    return new RPCClient(credentials, bitcoindUri, Network.TestNet);
});

var app = builder.Build();

app.MapFastEndpoints();

app.Run();