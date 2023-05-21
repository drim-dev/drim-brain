using System.Text;
using System.Text.Json;
using Domain;
using GenericHost.Kestrel.Endpoints;
using GenericHost.Kestrel.Endpoints.Controllers;
using GenericHost.Kestrel.Endpoints.Controllers.Extensions;
using GenericHost.Kestrel.Endpoints.Dtos;
using GenericHost.Kestrel.Endpoints.Endpoints;
using GenericHost.Kestrel.Endpoints.HostedServices;
using GenericHost.Kestrel.Endpoints.Middlewares;
using GenericHost.Kestrel.Endpoints.Middlewares.Terminal;
using GenericHost.Kestrel.Endpoints.Pipeline;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Services.Configuration;
using Services.Configuration.Options;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<NewDepositHostedService>();
        services.AddHostedService<DepositConfirmationsHostedService>();
        services.AddHostedService<KestrelHostedService>();

        services.AddScoped<DbContext>();

        services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
        services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

        services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
        services.AddTransient<IDepositRepository, DepositRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();

        services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();

        services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();

        services.Configure<BitcoinNodeClientOptions>(hostContext.Configuration.GetSection("BitcoinNodeClient"));
        services.Configure<NewDepositProcessingOptions>(hostContext.Configuration.GetSection("NewDepositProcessing"));
        services.Configure<DepositConfirmationsProcessingOptions>(hostContext.Configuration.GetSection("DepositConfirmationsProcessing"));
    })
    .AddPipeline(builder => builder
        .Use<LoggingMiddleware>()
        .Use<ExceptionPageMiddleware>()
        .Use<StaticFilesMiddleware>()
        .Use<RoutingMiddleware>()
        .Use<RateLimitingMiddleware>()
        .Use<EndpointExecutionMiddleware>()
        .UseEndpoint("/exception", (context, scope) =>
        {
            throw new Exception("You hit the exception route");
        })
        // .UseEndpoint("/deposits", async (context, scope) =>
        // {
        //     var depositRepository = scope.ServiceProvider.GetRequiredService<IDepositRepository>();
        //
        //     var depositModels = (await depositRepository.LoadAllDeposits(CancellationToken.None))
        //         .Select(x => new DepositDto
        //         {
        //             UserId = x.UserId,
        //             Currency = x.Currency,
        //             Amount = x.Amount,
        //             IsConfirmed = x.IsConfirmed,
        //         });
        //
        //     var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        //     var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;
        //
        //     responseFeature.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
        //     await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));
        // }, new Dictionary<string, object>{{EndpointMetadataKeys.RateLimitingInverval, 10_000}})
        .UseControllerEndpoints()
        .UseEndpoint("/health", async (context, scope) =>
        {
            var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
            var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

            responseFeature.Headers.Add("Content-Type", new StringValues("text/plain; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync("OK"u8.ToArray());
        }, new Dictionary<string, object>{{EndpointMetadataKeys.RateLimitingInverval, 5_000}}))
    .Build();

await host.RunAsync();
