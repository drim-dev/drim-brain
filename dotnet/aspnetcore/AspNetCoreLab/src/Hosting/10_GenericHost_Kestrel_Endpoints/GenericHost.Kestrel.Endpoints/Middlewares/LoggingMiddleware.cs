using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Endpoints.Middlewares;

public class LoggingMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("RequestResponseLogger");

        logger.LogInformation("Request: {Method} {Path}", requestFeature.Method, requestFeature.Path);
        var stopwatch = Stopwatch.StartNew();

        await next();

        stopwatch.Stop();
        logger.LogInformation("Response: {Method} {Path} HTTP {StatusCode} {ElapsedMilliseconds}ms",
            requestFeature.Method, requestFeature.Path, responseFeature.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
