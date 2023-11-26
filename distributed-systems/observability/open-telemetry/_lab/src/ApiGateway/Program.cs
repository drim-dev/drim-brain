using Common.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<Clients.BankingService.Withdrawals.WithdrawalsClient>(o =>
{
    o.Address = new Uri("http://localhost:5028");
});

var app = builder.Build();

app.MapEndpoints();

app.Run();
