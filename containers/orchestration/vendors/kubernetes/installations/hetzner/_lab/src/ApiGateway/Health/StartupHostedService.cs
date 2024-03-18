namespace ApiGateway.Health;

public class StartupHostedService(ReadinessHealthCheck _healthCheck) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10000, stoppingToken);

        _healthCheck.IsReady = true;
    }
}
