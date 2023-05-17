using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Services.Configuration;

namespace GenericHost.Kestrel;

public class KestrelHostedService : IHostedService
{
    private const int Port = 8080;

    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private KestrelServer? _server;

    public KestrelHostedService(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory)
    {
        _loggerFactory = loggerFactory;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var transportOptions = new OptionsWrapper<SocketTransportOptions>(new SocketTransportOptions());
        var transportFactory = new SocketTransportFactory(transportOptions, _loggerFactory);

        var serverOptions = new OptionsWrapper<KestrelServerOptions>(new());
        serverOptions.Value.ListenAnyIP(Port);

        _server = new KestrelServer(serverOptions, transportFactory, _loggerFactory);

        await _server.StartAsync(new HttpApplication(_serviceScopeFactory), cancellationToken);
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

internal record HttpApplicationContext(IFeatureCollection Features);

internal class HttpApplication : IHttpApplication<HttpApplicationContext>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public HttpApplication(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
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

        var depositRepository = scope.ServiceProvider.GetRequiredService<IDepositRepository>();

        var depositModels = (await depositRepository.LoadAllDeposits(CancellationToken.None))
            .Select(x => new DepositDto
            {
                UserId = x.UserId,
                Currency = x.Currency,
                Amount = x.Amount,
                IsConfirmed = x.IsConfirmed,
            });

        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (requestFeature.Path == "/deposits")
        {
            responseFeature.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));
        }
        else
        {
            responseFeature.StatusCode = StatusCodes.Status404NotFound;
        }

        //No HttpContext needed
        // HttpContext httpContext = new DefaultHttpContext(context.Features);
        // httpContext.Response.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
        // await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));
    }
}

public class DepositDto
{
    public int UserId { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public bool IsConfirmed { get; set; }
}
