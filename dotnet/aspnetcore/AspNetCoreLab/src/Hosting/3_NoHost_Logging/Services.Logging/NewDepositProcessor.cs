using Microsoft.Extensions.Logging;

namespace Services.Logging;

public interface INewDepositProcessor
{
    Task Process(CancellationToken cancellationToken);
}

public class NewDepositProcessor : INewDepositProcessor
{
    private readonly IDepositAddressRepository _depositAddressRepository;
    private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly IDepositRepository _depositRepository;
    private readonly ILogger<NewDepositProcessor> _logger;

    public NewDepositProcessor(
        IDepositAddressRepository depositAddressRepository,
        IBitcoinBlockchainScanner bitcoinBlockchainScanner,
        IDepositRepository depositRepository,
        ILogger<NewDepositProcessor> logger)
    {
        _depositAddressRepository = depositAddressRepository;
        _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
        _depositRepository = depositRepository;
        _logger = logger;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        _logger.LogInformation("New deposit processing started");

        var depositAddresses = await _depositAddressRepository.LoadDepositAddresses(cancellationToken);

        var deposits = await _bitcoinBlockchainScanner.FindNewDeposits(depositAddresses, cancellationToken);

        await _depositRepository.SaveNewDeposits(deposits, cancellationToken);

        _logger.LogInformation("New deposit processing finished");
    }
}