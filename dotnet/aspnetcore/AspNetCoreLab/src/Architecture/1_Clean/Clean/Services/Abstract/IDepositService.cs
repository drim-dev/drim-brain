using Clean.Models;

namespace Clean.Services.Abstract;

public interface IDepositService
{
    Task<DepositModel[]> GetDeposits(int userId, CancellationToken cancellationToken);
    Task<DepositAddressModel> GetDepositAddress(int userId, CancellationToken cancellationToken);
}
