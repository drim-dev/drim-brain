using GenericHost.Kestrel.Endpoints.Middlewares;

namespace GenericHost.Kestrel.Endpoints;

public interface IPipelineBuilder
{
    IPipelineBuilder Use<TMiddleware>() where TMiddleware : class, IPipelineMiddleware;
    IPipelineBuilder UseEndpoint(string pathPattern, Func<HttpApplicationContext, IServiceScope, Task> endpointDelegate, Dictionary<string, object>? metadata = null);
}

internal class PipelineBuilder : IPipelineBuilder
{
    private readonly List<Type> _middlewareTypes = new();
    private readonly EndpointCollection _endpointCollection = new();
    private readonly IServiceCollection _services;

    public PipelineBuilder(IServiceCollection services)
    {
        _services = services;
        _services.AddSingleton(_endpointCollection);
    }

    public IPipelineBuilder Use<TMiddleware>() where TMiddleware : class, IPipelineMiddleware
    {
        _services.AddTransient<TMiddleware>();
        _middlewareTypes.Add(typeof(TMiddleware));
        return this;
    }

    public IPipelineBuilder UseEndpoint(string pathPattern, Func<HttpApplicationContext, IServiceScope, Task> endpointDelegate, Dictionary<string, object>? metadata = null)
    {
        var endpoint = new Endpoint(pathPattern, endpointDelegate, metadata);
        _endpointCollection.Add(endpoint);
        return this;
    }

    public Pipeline Build()
    {
        return new Pipeline(_middlewareTypes);
    }
}