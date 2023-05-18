using System.Collections.Concurrent;
using GenericHost.Kestrel.Endpoints.Endpoints;
using GenericHost.Kestrel.Endpoints.HostedServices;
using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Endpoints.Middlewares;

public class RateLimitingMiddleware : IPipelineMiddleware
{
    private static readonly ConcurrentDictionary<string, DateTime> LastExecutionTimes = new();

    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var endpoint = context.Features.Get<EndpointFeature>()!.Endpoint!;
        if (endpoint.Metadata is null || !endpoint.Metadata.TryGetValue(EndpointMetadataKeys.RateLimitingInverval, out var interval))
        {
            await next();
            return;
        }
        var lastExecutionTime = LastExecutionTimes.GetOrAdd(endpoint.PathPattern, _ => DateTime.MinValue);
        if (DateTime.UtcNow - lastExecutionTime < TimeSpan.FromMilliseconds((int)interval))
        {
            var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
            responseFeature.StatusCode = StatusCodes.Status429TooManyRequests;
        }
        else
        {
            LastExecutionTimes[endpoint.PathPattern] = DateTime.UtcNow;
            await next();
        }
    }
}
