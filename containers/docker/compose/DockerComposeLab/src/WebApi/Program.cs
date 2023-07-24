using System.Reflection;
using FastEndpoints;
using WebApi.Database.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddFastEndpoints();

builder.AddDatabase();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
