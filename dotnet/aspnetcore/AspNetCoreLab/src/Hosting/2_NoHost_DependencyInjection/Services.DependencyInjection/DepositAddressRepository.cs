using Domain;

namespace Services.DependencyInjection;

public interface IDepositAddressRepository
{
    Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken);
}

public class DepositAddressRepository : IDepositAddressRepository
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