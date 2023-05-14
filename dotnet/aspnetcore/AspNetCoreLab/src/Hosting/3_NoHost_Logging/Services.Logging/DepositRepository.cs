using Domain;
using Microsoft.Extensions.Logging;

namespace Services.Logging;

public interface IDepositRepository
{
    Task SaveNewDeposits(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
    Task<IEnumerable<Deposit>> LoadUnconfirmedDeposits(CancellationToken cancellationToken);
    Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
}

public class DepositRepository : IDepositRepository
{
    private readonly DbContext _dbContext;
    private readonly ILogger<DepositRepository> _logger;
    private readonly int _minConfirmations = 3;

    public DepositRepository(
        DbContext dbContext,
        ILogger<DepositRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task SaveNewDeposits(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        _logger.LogInformation("New deposits saved");

        return Task.CompletedTask;
    }

    public Task<IEnumerable<Deposit>> LoadUnconfirmedDeposits(CancellationToken cancellationToken)
    {
        IEnumerable<Deposit> deposits = new[] { new Deposit(), new Deposit() };

        _logger.LogInformation("Unconfirmed deposits loaded");

        return Task.FromResult(deposits);
    }

    public Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deposit confirmations updated in database");

        return Task.CompletedTask;
    }
}
