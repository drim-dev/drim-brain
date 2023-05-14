using Domain;
using GenericHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Configuration;
using Services.Configuration.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<LifetimeHostedService>();
        services.AddHostedService<BitcoinNodeConnectionHostedService>();
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

        services.Configure<BitcoinNodeClientOptions>(hostContext.Configuration.GetSection("BitcoinNodeClient"));
        services.Configure<NewDepositProcessingOptions>(hostContext.Configuration.GetSection("NewDepositProcessing"));
        services.Configure<DepositConfirmationsProcessingOptions>(hostContext.Configuration.GetSection("DepositConfirmationsProcessing"));
    })
    .ConfigureLogging((context, builder) =>
    {
        // Configure existing logging providers
        // Add custom logging providers
    })
    .ConfigureAppConfiguration(builder =>
    {
        // Configure existing configuration providers
        // Add custom configuration providers
    })
    .Build();

await host.RunAsync();