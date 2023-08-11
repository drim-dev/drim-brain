using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Logging;

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

// using var loggerFactory = LoggerFactory.Create(builder =>
// {
//     builder.AddConsole();
// });
//
// services.AddSingleton(loggerFactory);
// services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

// TODO: uncomment
services.AddLogging(builder =>
{
    builder.AddConsole();
});

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

            await Task.Delay(TimeSpan.FromSeconds(5));
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

            await Task.Delay(TimeSpan.FromSeconds(11));
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
