using Clean.Database;
using Clean.Domain;
using Clean.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Clean.Repositories;

public class DepositRepository : IDepositRepository
{
    private readonly CleanDbContext _db;

    public DepositRepository(CleanDbContext db)
    {
        _db = db;
    }

    public Task<Deposit[]> GetDepositsByUserId(int userId, CancellationToken cancellationToken)
    {
        return _db.Deposits
            .Where(x => x.UserId == userId)
            .ToArrayAsync(cancellationToken);
    }
}
