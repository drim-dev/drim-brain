using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace WebApplicationApi.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;

        _logger.LogInformation("Request: {Method} {Path}", requestFeature.Method, requestFeature.Path);
        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();
        _logger.LogInformation("Response: {Method} {Path} HTTP {StatusCode} {ElapsedMilliseconds}ms",
            requestFeature.Method, requestFeature.Path, responseFeature.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}

public static class LoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}