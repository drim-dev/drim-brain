using Microsoft.Extensions.Options;
using Vertical.Features.Deposits.Options;
using Vertical.Pipeline;

namespace Vertical.Features.Deposits.Jobs.HostedServices;

public class UpdateDepositConfirmationsHostedService : BackgroundService
{
    private readonly Dispatcher _dispatcher;
    private readonly DepositsOptions _options;
    private readonly ILogger<UpdateDepositConfirmationsHostedService> _logger;

    public UpdateDepositConfirmationsHostedService(
        Dispatcher dispatcher,
        IOptions<DepositsOptions> options,
        ILogger<UpdateDepositConfirmationsHostedService> logger)
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
                await _dispatcher.Dispatch(new UpdateDepositConfirmations.Job(), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update deposit confirmations");
            }

            await Task.Delay(_options.UpdateDepositConfirmationsInterval, stoppingToken);
        }
    }
}
