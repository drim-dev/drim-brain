using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services.Configuration;

public interface IDepositRepository
{
    Task SaveNewDeposits(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
    Task<IEnumerable<Deposit>> LoadUnconfirmedDeposits(CancellationToken cancellationToken);
    Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
    Task<IEnumerable<Deposit>> LoadAllDeposits(CancellationToken cancellationToken);
}

public class DepositRepository : IDepositRepository
{
    private readonly DbContext _dbContext;
    private readonly ILogger<DepositRepository> _logger;
    private readonly int _minConfirmations = 3;

    public DepositRepository(
        DbContext dbContext,
        IConfiguration configuration,
        ILogger<DepositRepository> logger)
    {
        _dbContext = dbContext;
        _minConfirmations = configuration.GetSection("DepositConfirmationsProcessing:MinConfirmations").Get<int>();
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

    public Task<IEnumerable<Deposit>> LoadAllDeposits(CancellationToken cancellationToken)
    {
        IEnumerable<Deposit> deposits = new[]
        {
            new Deposit { UserId = 1, Currency = "BTC", Amount = 2.5m, IsConfirmed = true },
            new Deposit { UserId = 1, Currency = "BTC", Amount = 0.75m, IsConfirmed = false },
        };

        return Task.FromResult(deposits);
    }
}
