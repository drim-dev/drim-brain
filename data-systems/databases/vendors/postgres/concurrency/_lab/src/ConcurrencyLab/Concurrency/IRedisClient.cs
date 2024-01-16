namespace Concurrency.Concurrency;

public interface IRedisClient
{
    Task<IAsyncDisposable> ObtainLock(string name, CancellationToken cancellationToken);
}
