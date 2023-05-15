using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenericHost.Multiple;

public class HostedService1 : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<HostedService1> _logger;

    public HostedService1(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<HostedService1> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HostedService1 started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HostedService1 stopped");
        return Task.CompletedTask;
    }
}