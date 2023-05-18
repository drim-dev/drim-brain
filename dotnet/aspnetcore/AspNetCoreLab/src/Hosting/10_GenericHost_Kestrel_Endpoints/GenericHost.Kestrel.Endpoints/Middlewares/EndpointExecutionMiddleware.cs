using GenericHost.Kestrel.Endpoints.Endpoints;
using GenericHost.Kestrel.Endpoints.HostedServices;

namespace GenericHost.Kestrel.Endpoints.Middlewares;

public class EndpointExecutionMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        await context.Features.Get<EndpointFeature>()!.Endpoint!.EndpointDelegate(context, scope);
    }
}
