using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Middlewares.Middlewares.Terminal;

public class NotFoundMiddleware : IPipelineMiddleware
{
    public Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;

        responseFeature.StatusCode = StatusCodes.Status404NotFound;

        return Task.CompletedTask;
    }
}
