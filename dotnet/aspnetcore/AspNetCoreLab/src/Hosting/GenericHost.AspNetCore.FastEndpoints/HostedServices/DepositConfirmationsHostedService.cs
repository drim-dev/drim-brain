using Microsoft.Extensions.Options;
using Services.Configuration;
using Services.Configuration.Options;

namespace GenericHost.AspNetCore.FastEndpoints.HostedServices;

public class DepositConfirmationsHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DepositConfirmationsHostedService> _logger;

    public DepositConfirmationsHostedService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DepositConfirmationsHostedService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();

                var options = scope.ServiceProvider.GetRequiredService<IOptions<DepositConfirmationsProcessingOptions>>().Value;

                await Task.Delay(options.Interval, stoppingToken);

                var depositConfirmationsProcessor = scope.ServiceProvider.GetRequiredService<IDepositConfirmationsProcessor>();

                await depositConfirmationsProcessor.Process(timeoutCts.Token);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutCts.Token)
            {
                _logger.LogError(ex, "Deposit confirmations processing timed out");
            }
        }
    }
}