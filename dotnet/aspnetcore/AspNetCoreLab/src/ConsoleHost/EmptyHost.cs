using Microsoft.Extensions.Hosting;

namespace ConsoleHost;

public static class EmptyHost
{
    static async Task EmptyHostMain(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args).Build();

        await host.RunAsync();
    }
}