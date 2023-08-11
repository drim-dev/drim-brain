using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Configuration;
using Services.Configuration.Options;

namespace GenericHost;

public class NewDepositHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<NewDepositHostedService> _logger;
    private readonly NewDepositProcessingOptions _options;

    public NewDepositHostedService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<NewDepositProcessingOptions> options,
        ILogger<NewDepositHostedService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
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

                await Task.Delay(_options.Interval, stoppingToken);

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
