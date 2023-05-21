using Clean.Domain;

namespace Clean.Repositories.Abstract;

public interface IDepositAddressRepository
{
    Task<DepositAddress?> GetDepositAddressByUserId(int userId, CancellationToken cancellationToken);
    void Add(DepositAddress address);
}
