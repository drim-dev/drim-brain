namespace Services.DependencyInjection;

public interface IDepositConfirmationsProcessor
{
    Task Process(CancellationToken cancellationToken);
}

public class DepositConfirmationsProcessor : IDepositConfirmationsProcessor
{
    private readonly IDepositRepository _depositRepository;
    private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly IAccountRepository _accountRepository;

    public DepositConfirmationsProcessor(
        IDepositRepository depositRepository,
        IBitcoinBlockchainScanner bitcoinBlockchainScanner,
        IAccountRepository accountRepository)
    {
        _depositRepository = depositRepository;
        _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
        _accountRepository = accountRepository;
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