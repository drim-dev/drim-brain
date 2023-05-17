namespace GenericHost.Kestrel.Middlewares.Middlewares;

public interface IPipelineMiddleware
{
    Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next);
}
