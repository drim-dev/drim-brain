using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace GenericHost.Kestrel.Middlewares.Middlewares.Terminal;

public class HealthCheckMiddleware : IPipelineMiddleware
{
    private static DateTime _previousHealthCheckRequestTime = DateTime.MinValue;

    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (!requestFeature.Path.Equals("/health", StringComparison.InvariantCultureIgnoreCase))
        {
            await next();
            return;
        }

        if (DateTime.UtcNow - _previousHealthCheckRequestTime < TimeSpan.FromSeconds(5))
        {
            responseFeature.StatusCode = StatusCodes.Status429TooManyRequests;
        }
        else
        {
            responseFeature.Headers.Add("Content-Type", new StringValues("text/plain; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync("OK"u8.ToArray());

            _previousHealthCheckRequestTime = DateTime.UtcNow;
        }
    }
}
