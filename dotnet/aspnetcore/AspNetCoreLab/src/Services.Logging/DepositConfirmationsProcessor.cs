using Microsoft.Extensions.Logging;

namespace Services.Logging;

public interface IDepositConfirmationsProcessor
{
    Task Process(CancellationToken cancellationToken);
}

public class DepositConfirmationsProcessor : IDepositConfirmationsProcessor
{
    private readonly IDepositRepository _depositRepository;
    private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<DepositConfirmationsProcessor> _logger;

    public DepositConfirmationsProcessor(
        IDepositRepository depositRepository,
        IBitcoinBlockchainScanner bitcoinBlockchainScanner,
        IAccountRepository accountRepository,
        ILogger<DepositConfirmationsProcessor> logger)
    {
        _depositRepository = depositRepository;
        _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deposit confirmations processing started");

        var unconfirmedDeposits = await _depositRepository.LoadUnconfirmedDeposits(cancellationToken);

        await _bitcoinBlockchainScanner.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

        await _depositRepository.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

        var confirmedDeposits = unconfirmedDeposits.Where(d => d.IsConfirmed);

        await _accountRepository.DepositToAccounts(confirmedDeposits, cancellationToken);

        _logger.LogInformation("Deposit confirmations processing finished");
    }
}