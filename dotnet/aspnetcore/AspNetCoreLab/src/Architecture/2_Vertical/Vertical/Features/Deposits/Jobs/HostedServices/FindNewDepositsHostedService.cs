using Microsoft.Extensions.Options;
using Vertical.Features.Deposits.Options;
using Vertical.Pipeline;

namespace Vertical.Features.Deposits.Jobs.HostedServices;

public class FindNewDepositsHostedService : BackgroundService
{
    private readonly Dispatcher _dispatcher;
    private readonly DepositsOptions _options;
    private readonly ILogger<FindNewDepositsHostedService> _logger;

    public FindNewDepositsHostedService(
        Dispatcher dispatcher,
        IOptions<DepositsOptions> options,
        ILogger<FindNewDepositsHostedService> logger)
    {
        _dispatcher = dispatcher;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _dispatcher.Dispatch(new FindNewDeposits.Job(), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find new deposits");
            }

            await Task.Delay(_options.FindNewDepositsInterval, stoppingToken);
        }
    }
}
