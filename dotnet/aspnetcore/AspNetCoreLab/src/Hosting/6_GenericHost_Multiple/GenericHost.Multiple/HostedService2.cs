using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenericHost.Multiple;

public class HostedService2 : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<HostedService2> _logger;

    public HostedService2(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<HostedService2> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;

        _hostApplicationLifetime.ApplicationStarted.Register(() => _logger.LogInformation("Application started"));
        _hostApplicationLifetime.ApplicationStopping.Register(() => _logger.LogInformation("Application stopping"));
        _hostApplicationLifetime.ApplicationStopped.Register(() => _logger.LogInformation("Application stopped"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        _logger.LogInformation("Gracefully stopping the application");

        _hostApplicationLifetime.StopApplication();
    }
}