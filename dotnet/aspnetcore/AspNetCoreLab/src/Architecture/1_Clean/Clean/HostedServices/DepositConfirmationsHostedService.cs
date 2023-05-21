using Clean.Options;
using Microsoft.Extensions.Options;
using Services.Configuration;

namespace Clean.HostedServices;

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
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();

                var options = scope.ServiceProvider.GetRequiredService<IOptions<DepositsOptions>>().Value;

                var depositConfirmationsProcessor = scope.ServiceProvider.GetRequiredService<IDepositConfirmationsProcessor>();

                await depositConfirmationsProcessor.Process(stoppingToken);

                await Task.Delay(options.UpdateDepositConfirmationsInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update deposit confirmations");
            }
        }
    }
}
