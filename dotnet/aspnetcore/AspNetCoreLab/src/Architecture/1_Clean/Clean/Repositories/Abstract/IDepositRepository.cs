using Clean.Domain;

namespace Clean.Repositories.Abstract;

public interface IDepositRepository
{
    Task<Deposit[]> GetDepositsByUserId(int userId, CancellationToken cancellationToken);
}
