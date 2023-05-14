using Domain;

namespace Services.Raw;

public class DepositAddressRepository
{
    private readonly DbContext _dbContext;

    public DepositAddressRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken)
    {
        IEnumerable<DepositAddress> addresses = new[] { new DepositAddress(), new DepositAddress() };

        Console.WriteLine("Deposit addresses loaded");

        return Task.FromResult(addresses);
    }
}