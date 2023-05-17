using GenericHost.Kestrel.Middlewares;
using GenericHost.Kestrel.Middlewares.Middlewares;

public interface IPipelineBuilder
{
    IPipelineBuilder Use<TMiddleware>() where TMiddleware : class, IPipelineMiddleware;
}

internal class PipelineBuilder : IPipelineBuilder
{
    private readonly List<Type> _middlewareTypes = new();
    private readonly IServiceCollection _services;

    public PipelineBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IPipelineBuilder Use<TMiddleware>() where TMiddleware : class, IPipelineMiddleware
    {
        _services.AddTransient<TMiddleware>();
        _middlewareTypes.Add(typeof(TMiddleware));
        return this;
    }

    public Pipeline Build()
    {
        return new Pipeline(_middlewareTypes);
    }
}
