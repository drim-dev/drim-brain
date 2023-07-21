namespace Testing.Tests.Integration.Advanced.Collections.Helpers;

public static class Create
{
    public static CancellationToken CancellationToken(int timeoutInSeconds = 30) =>
        new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
}
