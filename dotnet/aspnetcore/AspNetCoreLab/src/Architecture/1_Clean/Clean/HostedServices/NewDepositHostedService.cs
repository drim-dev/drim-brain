using Clean.Options;
using Microsoft.Extensions.Options;
using Services.Configuration;

namespace Clean.HostedServices;

public class NewDepositHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<NewDepositHostedService> _logger;

    public NewDepositHostedService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NewDepositHostedService> logger)
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

                var newDepositProcessor = scope.ServiceProvider.GetRequiredService<INewDepositProcessor>();

                await newDepositProcessor.Process(stoppingToken);

                await Task.Delay(options.FindNewDepositsInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find new deposits");
            }
        }
    }
}
