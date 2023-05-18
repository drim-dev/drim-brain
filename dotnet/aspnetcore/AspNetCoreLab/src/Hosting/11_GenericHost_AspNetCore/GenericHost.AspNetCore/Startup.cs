using Domain;
using GenericHost.AspNetCore.Controllers.Attributes;
using GenericHost.AspNetCore.HostedServices;
using GenericHost.AspNetCore.Middlewares;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Services.Configuration;
using Services.Configuration.Options;

namespace GenericHost.AspNetCore;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<NewDepositHostedService>();
        services.AddHostedService<DepositConfirmationsHostedService>();

        services.AddScoped<DbContext>();

        services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
        services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

        services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
        services.AddTransient<IDepositRepository, DepositRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();

        services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();

        services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();

        services.Configure<BitcoinNodeClientOptions>(_configuration.GetSection("BitcoinNodeClient"));
        services.Configure<NewDepositProcessingOptions>(_configuration.GetSection("NewDepositProcessing"));
        services.Configure<DepositConfirmationsProcessingOptions>(_configuration.GetSection("DepositConfirmationsProcessing"));

        services.AddControllers();
        services.AddScoped<LoggingMiddleware>();
        services.AddScoped<RateLimitingMiddleware>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        app.UseLogging();

        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "public")),
            RequestPath = "/public"
        });

        app.UseRouting();

        app.UseRateLimiting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.Map("/exception", context =>
            {
                throw new Exception("You hit the exception route");
            });

            endpoints.Map("/health", async context =>
            {
                context.Response.Headers.Add("Content-Type", new StringValues("text/plain; charset=UTF-8"));
                await context.Response.Body.WriteAsync("OK"u8.ToArray());
            })
                .WithMetadata(new RateLimitingAttribute(5_000));

            endpoints.MapControllers();
        });
    }
}
