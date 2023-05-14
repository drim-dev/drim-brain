using Domain;

namespace Services.DependencyInjection;

public interface IAccountRepository
{
    Task DepositToAccounts(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
}

public class AccountRepository : IAccountRepository
{
    private readonly DbContext _dbContext;

    public AccountRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task DepositToAccounts(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        Console.WriteLine("Accounts deposited");

        return Task.CompletedTask;
    }
}