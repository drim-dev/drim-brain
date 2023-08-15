using Microsoft.Extensions.Options;
using Services.Configuration;
using Services.Configuration.Options;

namespace GenericHost.AspNetCore.FastEndpoints.HostedServices;

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
            var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();

                var options = scope.ServiceProvider.GetRequiredService<IOptions<NewDepositProcessingOptions>>().Value;

                await Task.Delay(options.Interval, stoppingToken);

                var newDepositProcessor = scope.ServiceProvider.GetRequiredService<INewDepositProcessor>();

                await newDepositProcessor.Process(timeoutCts.Token);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutCts.Token)
            {
                _logger.LogError(ex, "New deposits processing timed out");
            }
        }
    }
}
