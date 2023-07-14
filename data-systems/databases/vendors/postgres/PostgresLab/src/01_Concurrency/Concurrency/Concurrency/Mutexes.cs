namespace Transactions.Concurrency;

public static class Mutexes
{
    public static readonly Mutex AccountsMutex = new(false);
    public static readonly Mutex AccountsMutexNamed = new(false, "OsLevelAccountsMutex");
}
