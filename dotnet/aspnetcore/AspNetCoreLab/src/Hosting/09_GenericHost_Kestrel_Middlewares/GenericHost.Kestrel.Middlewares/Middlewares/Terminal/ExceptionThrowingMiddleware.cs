using GenericHost.Kestrel.Middlewares.HostedServices;
using GenericHost.Kestrel.Middlewares.Middlewares.Abstract;
using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Middlewares.Middlewares.Terminal;

public class ExceptionThrowingMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;

        if (!requestFeature.Path.Equals("/exception", StringComparison.InvariantCultureIgnoreCase))
        {
            await next();
            return;
        }

        throw new Exception("You hit the exception route");
    }
}
