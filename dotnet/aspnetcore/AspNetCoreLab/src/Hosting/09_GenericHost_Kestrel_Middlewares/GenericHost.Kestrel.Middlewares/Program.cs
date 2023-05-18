using Domain;
using GenericHost.Kestrel.Middlewares;
using GenericHost.Kestrel.Middlewares.Middlewares;
using GenericHost.Kestrel.Middlewares.Middlewares.Terminal;
using Services.Configuration;
using Services.Configuration.Options;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<NewDepositHostedService>();
        services.AddHostedService<DepositConfirmationsHostedService>();
        services.AddHostedService<KestrelHostedServicePipeline>();

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
        .Use<ExceptionThrowingMiddleware>()
        .Use<DepositsMiddleware>()
        .Use<HealthCheckMiddleware>()
        .Use<NotFoundMiddleware>())
    .Build();

await host.RunAsync();
