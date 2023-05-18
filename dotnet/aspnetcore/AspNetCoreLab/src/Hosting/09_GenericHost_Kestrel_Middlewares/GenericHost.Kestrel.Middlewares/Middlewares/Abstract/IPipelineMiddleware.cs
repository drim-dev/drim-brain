using GenericHost.Kestrel.Middlewares.HostedServices;

namespace GenericHost.Kestrel.Middlewares.Middlewares.Abstract;

public interface IPipelineMiddleware
{
    Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next);
}
