using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Configuration;
using Services.Configuration.Options;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, args) =>
{
    cts.Cancel();
    args.Cancel = true;
};

var services = new ServiceCollection();

services.AddScoped<DbContext>();

services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
services.AddTransient<IDepositRepository, DepositRepository>();
services.AddTransient<IAccountRepository, AccountRepository>();

services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();

services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();

services.AddLogging(builder => builder.AddConsole());

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

services.AddSingleton(configuration);

// TODO: uncomment
services.Configure<BitcoinNodeClientOptions>(configuration.GetSection("BitcoinNodeClient"));
services.Configure<NewDepositProcessingOptions>(configuration.GetSection("NewDepositProcessing"));
services.Configure<DepositConfirmationsProcessingOptions>(configuration.GetSection("DepositConfirmationsProcessing"));

var serviceProvider = services.BuildServiceProvider();

var programLogger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Program");

var newDepositsTask = Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var newDepositProcessor = scope.ServiceProvider.GetRequiredService<INewDepositProcessor>();

            await newDepositProcessor.Process(timeoutCts.Token);

            var options = scope.ServiceProvider.GetRequiredService<IOptions<NewDepositProcessingOptions>>();

            await Task.Delay(options.Value.Interval);
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutCts.Token)
        {
            programLogger.LogError(ex, "New deposits processing timed out");
        }
    }
});

var depositConfirmationsTask = Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var depositConfirmationsProcessor = scope.ServiceProvider.GetRequiredService<IDepositConfirmationsProcessor>();

            await depositConfirmationsProcessor.Process(timeoutCts.Token);

            var interval = configuration.GetSection("DepositConfirmationsProcessing:Interval").Get<TimeSpan>();

            await Task.Delay(interval);
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutCts.Token)
        {
            programLogger.LogError(ex, "Deposit confirmations processing timed out");
        }
    }
});

programLogger.LogInformation("Program started. Press Ctrl+C to exit");

await Task.WhenAll(newDepositsTask, depositConfirmationsTask);

programLogger.LogInformation("Program stopped");
