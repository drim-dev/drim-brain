using Domain;

namespace Services.Raw;

public class DepositConfirmationsProcessor
{
    private readonly DepositRepository _depositRepository;
    private readonly BitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly AccountRepository _accountRepository;

    public DepositConfirmationsProcessor(BitcoinNodeClient bitcoinNodeClient)
    {
        var sharedDbContext = new DbContext();
        _depositRepository = new(sharedDbContext);
        _bitcoinBlockchainScanner = new(bitcoinNodeClient);
        _accountRepository = new(sharedDbContext);
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        Console.WriteLine("Deposit confirmations processing started");

        var unconfirmedDeposits = await _depositRepository.LoadUnconfirmedDeposits(cancellationToken);

        await _bitcoinBlockchainScanner.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

        await _depositRepository.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

        var confirmedDeposits = unconfirmedDeposits.Where(d => d.IsConfirmed);

        await _accountRepository.DepositToAccounts(confirmedDeposits, cancellationToken);

        Console.WriteLine("Deposit confirmations processing finished");
    }
}