namespace Errors.Storages;

public static class AccountStorage
{
    private static readonly string[] AccountNumbers =
    {
        "KZ11111",
        "KZ22222",
        "KZ33333",
    };

    public static Task<bool> Exists(string accountNumber, CancellationToken cancellationToken)
    {
        var exists = AccountNumbers.Contains(accountNumber);
        return Task.FromResult(exists);
    }
}
