using Domain;
using Microsoft.Extensions.DependencyInjection;
using Services.DependencyInjection;

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

var serviceProvider = services.BuildServiceProvider();

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
            Console.WriteLine("New deposits processing timed out");
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
            Console.WriteLine("Deposit confirmations processing timed out");
        }
    }
});

Console.WriteLine("Program started. Press Ctrl+C to exit");

await Task.WhenAll(newDepositsTask, depositConfirmationsTask);

Console.WriteLine("Program stopped");
