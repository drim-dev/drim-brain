using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleHost;

public static class HostedServices
{
    static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<LoggingHostedService>();
                services.AddHostedService<BackgroundHostedService>();
            })
            .Build();

        await host.RunAsync();
    }
}

public class LoggingHostedService : IHostedService
{
    private readonly ILogger<LoggingHostedService> _logger;

    public LoggingHostedService(ILogger<LoggingHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LoggingHostedService is starting");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LoggingHostedService is stopping");

        return Task.CompletedTask;
    }
}

public class BackgroundHostedService : BackgroundService
{
    private readonly ILogger<BackgroundHostedService> _logger;

    public BackgroundHostedService(ILogger<BackgroundHostedService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("ThreadId: {ThreadId}", Environment.CurrentManagedThreadId);
            await Task.Delay(5_000, stoppingToken);
        }
    }
}
