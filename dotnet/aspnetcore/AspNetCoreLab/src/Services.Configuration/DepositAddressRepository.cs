using Domain;
using Microsoft.Extensions.Logging;

namespace Services.Configuration;

public interface IDepositAddressRepository
{
    Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken);
}

public class DepositAddressRepository : IDepositAddressRepository
{
    private readonly DbContext _dbContext;
    private readonly ILogger<DepositAddressRepository> _logger;

    public DepositAddressRepository(
        DbContext dbContext,
        ILogger<DepositAddressRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken)
    {
        IEnumerable<DepositAddress> addresses = new[] { new DepositAddress(), new DepositAddress() };

        _logger.LogInformation("Deposit addresses loaded");

        return Task.FromResult(addresses);
    }
}