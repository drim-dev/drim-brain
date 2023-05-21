using Clean.Database;
using Clean.Domain;
using Clean.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Clean.Repositories;

public class DepositAddressRepository : IDepositAddressRepository
{
    private readonly CleanDbContext _db;

    public DepositAddressRepository(CleanDbContext db)
    {
        _db = db;
    }

    public async Task<DepositAddress?> GetDepositAddressByUserId(int userId, CancellationToken cancellationToken)
    {
        return await _db.DepositAddresses.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public void Add(DepositAddress address)
    {
        _db.DepositAddresses.Add(address);
    }
}
