using Domain;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Services.Configuration;
using Services.Configuration.Options;
using WebApplicationApi.HostedServices;
using WebApplicationApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<NewDepositHostedService>();
builder.Services.AddHostedService<DepositConfirmationsHostedService>();

builder.Services.AddScoped<DbContext>();

builder.Services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
builder.Services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

builder.Services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
builder.Services.AddTransient<IDepositRepository, DepositRepository>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();

builder.Services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();

builder.Services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();

builder.Services.Configure<BitcoinNodeClientOptions>(builder.Configuration.GetSection("BitcoinNodeClient"));
builder.Services.Configure<NewDepositProcessingOptions>(builder.Configuration.GetSection("NewDepositProcessing"));
builder.Services.Configure<DepositConfirmationsProcessingOptions>(builder.Configuration.GetSection("DepositConfirmationsProcessing"));

builder.Services.AddControllers();
builder.Services.AddScoped<LoggingMiddleware>();

var app = builder.Build();

app.UseLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "public")),
    RequestPath = "/public"
});

app.MapGet("/exception", context =>
{
    throw new Exception("You hit the exception route");
});

app.MapGet("/health", async context =>
{
    context.Response.Headers.Add("Content-Type", new StringValues("text/plain; charset=UTF-8"));
    await context.Response.Body.WriteAsync("OK"u8.ToArray());
});

app.MapControllers();

app.Run();
