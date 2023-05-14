using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services.Configuration;

public interface IBitcoinBlockchainScanner
{
    Task<IEnumerable<Deposit>> FindNewDeposits(IEnumerable<DepositAddress> addresses, CancellationToken cancellationToken);
    Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
}

public class BitcoinBlockchainScanner : IBitcoinBlockchainScanner
{
    private readonly IBitcoinNodeClient _bitcoinNodeClient;
    private readonly ILogger<BitcoinBlockchainScanner> _logger;
    private readonly int _minConfirmations = 3;

    public BitcoinBlockchainScanner(
        IBitcoinNodeClient bitcoinNodeClient,
        IConfiguration configuration,
        ILogger<BitcoinBlockchainScanner> logger)
    {
        _bitcoinNodeClient = bitcoinNodeClient;
        _minConfirmations = configuration.GetSection("DepositConfirmationsProcessing:MinConfirmations").Get<int>();
        _logger = logger;
    }

    public Task<IEnumerable<Deposit>> FindNewDeposits(IEnumerable<DepositAddress> addresses, CancellationToken cancellationToken)
    {
        IEnumerable<Deposit> deposits = new[] { new Deposit(), new Deposit() };

        _logger.LogInformation("New deposits found");

        return Task.FromResult(deposits);
    }

    public Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deposit confirmations updated");

        return Task.CompletedTask;
    }
}
