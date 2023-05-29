namespace Errors.Storages;

public static class CurrencyStorage
{
    private static readonly string[] Currencies = { "BTC" };

    public static Task<bool> Exists(string currency, CancellationToken cancellationToken)
    {
        var exists = Currencies.Contains(currency);
        return Task.FromResult(exists);
    }
}
