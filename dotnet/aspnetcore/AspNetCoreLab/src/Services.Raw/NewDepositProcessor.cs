using Domain;

namespace Services.Raw;

public class NewDepositProcessor
{
    private readonly DepositAddressRepository _depositAddressRepository;
    private readonly BitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly DepositRepository _depositRepository;

    public NewDepositProcessor(BitcoinNodeClient bitcoinNodeClient)
    {
        var sharedDbContext = new DbContext();
        _depositAddressRepository = new(sharedDbContext);
        _bitcoinBlockchainScanner = new(bitcoinNodeClient);
        _depositRepository = new(sharedDbContext);
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