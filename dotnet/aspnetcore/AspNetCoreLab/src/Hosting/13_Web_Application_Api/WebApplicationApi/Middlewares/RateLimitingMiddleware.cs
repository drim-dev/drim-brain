using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.Features;
using WebApplicationApi.Controllers.Attributes;

namespace WebApplicationApi.Middlewares;

public class RateLimitingMiddleware : IMiddleware
{
    private static readonly ConcurrentDictionary<string, DateTime> LastExecutionTimes = new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()!.Endpoint!;
        var rateLimitingAttribute = endpoint.Metadata.GetMetadata<RateLimitingAttribute>();
        if (rateLimitingAttribute is null)
        {
            await next(context);
            return;
        }
        var lastExecutionTime = LastExecutionTimes.GetOrAdd(context.Request.Path, _ => DateTime.MinValue);
        if (DateTime.UtcNow - lastExecutionTime < TimeSpan.FromMilliseconds(rateLimitingAttribute.IntervalMs))
        {
            var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
            responseFeature.StatusCode = StatusCodes.Status429TooManyRequests;
        }
        else
        {
            LastExecutionTimes[context.Request.Path] = DateTime.UtcNow;
            await next(context);
        }
    }
}

public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}
