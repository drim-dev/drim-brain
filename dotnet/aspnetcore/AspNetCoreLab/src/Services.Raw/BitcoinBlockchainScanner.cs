using Domain;

namespace Services.Raw;

public class BitcoinBlockchainScanner
{
    private readonly BitcoinNodeClient _bitcoinNodeClient;
    private readonly int _minConfirmations = 3;

    public BitcoinBlockchainScanner(BitcoinNodeClient bitcoinNodeClient)
    {
        _bitcoinNodeClient = bitcoinNodeClient;
    }

    public Task<IEnumerable<Deposit>> FindNewDeposits(IEnumerable<DepositAddress> addresses, CancellationToken cancellationToken)
    {
        IEnumerable<Deposit> deposits = new[] { new Deposit(), new Deposit() };

        Console.WriteLine("New deposits found");

        return Task.FromResult(deposits);
    }

    public Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        Console.WriteLine("Deposit confirmations updated");

        return Task.CompletedTask;
    }
}