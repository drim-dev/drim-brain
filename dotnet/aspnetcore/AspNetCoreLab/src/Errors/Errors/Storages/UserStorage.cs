namespace Errors.Storages;

public static class UserStorage
{
    private static readonly (string name, string password)[] Users =
    {
        ("buyer", "buyer123456"),
        ("seller", "seller123456"),
        ("seller2", "seller123456"),
        ("seller5", "seller123456"),
        ("owner", "owner123456"),
    };

    public static Task<bool> Exists(string name, string password, CancellationToken cancellationToken)
    {
        var exists = Users.Any(x => x.name == name && x.password == password);
        return Task.FromResult(exists);
    }

    public static Task<bool> Exists(string name, CancellationToken cancellationToken)
    {
        var exists = Users.Any(x => x.name == name);
        return Task.FromResult(exists);
    }
}
