using Domain;

namespace Services.Raw;

public class DepositRepository
{
    private readonly DbContext _dbContext;
    private readonly int _minConfirmations = 3;

    public DepositRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveNewDeposits(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        Console.WriteLine("New deposits saved");

        return Task.CompletedTask;
    }

    public Task<IEnumerable<Deposit>> LoadUnconfirmedDeposits(CancellationToken cancellationToken)
    {
        IEnumerable<Deposit> deposits = new[] { new Deposit(), new Deposit() };

        Console.WriteLine("Unconfirmed deposits loaded");

        return Task.FromResult(deposits);
    }

    public Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        Console.WriteLine("Deposit confirmations updated in database");

        return Task.CompletedTask;
    }
}
