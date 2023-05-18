using GenericHost.Kestrel.Endpoints.Endpoints;
using GenericHost.Kestrel.Endpoints.HostedServices;
using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Endpoints.Middlewares;

public class RoutingMiddleware : IPipelineMiddleware
{
    private readonly EndpointCollection _endpoints;

    public RoutingMiddleware(EndpointCollection endpoints)
    {
        _endpoints = endpoints;
    }

    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;

        // TODO: implement full-blown routing
        foreach (var endpoint in _endpoints)
        {
            if (requestFeature.Path.Equals(endpoint.PathPattern, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Features.Set(new EndpointFeature(endpoint));
                await next();
                return;
            }
        }

        responseFeature.StatusCode = StatusCodes.Status404NotFound;
    }
}
