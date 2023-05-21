using MediatR;

namespace Vertical.Pipeline.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("LoggingBehavior");
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestType}", request.GetType().FullName);
        _logger.LogDebug("Request: {@Request}", request);

        var response = await next();

        _logger.LogInformation("Handled {RequestType}", request.GetType().FullName);
        _logger.LogDebug("Response: {@Request}", response);

        return response;
    }
}
