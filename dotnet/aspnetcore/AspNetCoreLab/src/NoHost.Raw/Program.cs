using Services.Raw;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, args) =>
{
    cts.Cancel();
    args.Cancel = true;
};

var bitcoinNodeClient = new BitcoinNodeClient();

var newDepositsTask = Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        try
        {
            var newDepositProcessor = new NewDepositProcessor(bitcoinNodeClient);

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
            var depositConfirmationsProcessor = new DepositConfirmationsProcessor(bitcoinNodeClient);

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
