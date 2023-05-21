namespace Clean.Database;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly CleanDbContext _db;

    public UnitOfWork(CleanDbContext db)
    {
        _db = db;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
