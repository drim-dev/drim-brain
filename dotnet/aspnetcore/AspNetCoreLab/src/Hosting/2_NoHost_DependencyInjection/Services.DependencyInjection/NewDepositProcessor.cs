namespace Services.DependencyInjection;

public interface INewDepositProcessor
{
    Task Process(CancellationToken cancellationToken);
}

public class NewDepositProcessor : INewDepositProcessor
{
    private readonly IDepositAddressRepository _depositAddressRepository;
    private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly IDepositRepository _depositRepository;

    public NewDepositProcessor(
        IDepositAddressRepository depositAddressRepository,
        IBitcoinBlockchainScanner bitcoinBlockchainScanner,
        IDepositRepository depositRepository)
    {
        _depositAddressRepository = depositAddressRepository;
        _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
        _depositRepository = depositRepository;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        Console.WriteLine("New deposit processing started");

        var depositAddresses = await _depositAddressRepository.LoadDepositAddresses(cancellationToken);

        var deposits = await _bitcoinBlockchainScanner.FindNewDeposits(depositAddresses, cancellationToken);

        await _depositRepository.SaveNewDeposits(deposits, cancellationToken);

        Console.WriteLine("New deposit processing finished");
    }
}