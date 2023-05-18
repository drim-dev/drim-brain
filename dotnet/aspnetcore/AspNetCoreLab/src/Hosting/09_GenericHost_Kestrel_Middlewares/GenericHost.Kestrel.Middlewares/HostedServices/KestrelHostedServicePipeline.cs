using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Options;

namespace GenericHost.Kestrel.Middlewares.HostedServices;

internal class KestrelHostedServicePipeline : IHostedService
{
    private const int Port = 8080;

    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Pipeline.Pipeline _pipeline;
    private KestrelServer? _server;

    public KestrelHostedServicePipeline(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        Pipeline.Pipeline pipeline)
    {
        _loggerFactory = loggerFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _pipeline = pipeline;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var transportOptions = new OptionsWrapper<SocketTransportOptions>(new SocketTransportOptions());
        var transportFactory = new SocketTransportFactory(transportOptions, _loggerFactory);

        var serverOptions = new OptionsWrapper<KestrelServerOptions>(new());
        serverOptions.Value.ListenAnyIP(Port);

        _server = new KestrelServer(serverOptions, transportFactory, _loggerFactory);

        await _server.StartAsync(new HttpApplication(_serviceScopeFactory, _pipeline), cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_server is not null)
        {
            await _server.StopAsync(cancellationToken);
            _server.Dispose();
        }
    }
}

internal class HttpApplication : IHttpApplication<HttpApplicationContext>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Pipeline.Pipeline _pipeline;

    public HttpApplication(
        IServiceScopeFactory serviceScopeFactory,
        Pipeline.Pipeline pipeline)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _pipeline = pipeline;
    }

    public HttpApplicationContext CreateContext(IFeatureCollection contextFeatures)
    {
        return new HttpApplicationContext(contextFeatures);
    }

    public void DisposeContext(HttpApplicationContext context, Exception? exception)
    {
    }

    public async Task ProcessRequestAsync(HttpApplicationContext context)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await _pipeline.Invoke(context, scope);
    }
}

public record HttpApplicationContext(IFeatureCollection Features);
