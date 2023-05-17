using GenericHost.Kestrel.Middlewares.Middlewares;

namespace GenericHost.Kestrel.Middlewares;

internal class Pipeline
{
    private readonly IEnumerable<Type> _middlewareTypes;

    public Pipeline(IEnumerable<Type> middlewareTypes)
    {
        _middlewareTypes = middlewareTypes;
    }

    public async Task Invoke(HttpApplicationContext context, AsyncServiceScope scope)
    {
        Func<Task> next = () => Task.CompletedTask;

        foreach (var middlewareType in _middlewareTypes.Reverse())
        {
            var middleware = (IPipelineMiddleware)scope.ServiceProvider.GetRequiredService(middlewareType);
            var nextMiddleware = next;
            next = () => middleware.Invoke(context, scope, nextMiddleware);
        }

        await next();
    }
}
