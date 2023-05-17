using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Services.Configuration;

namespace GenericHost.Kestrel.RequestProcessing;

public class KestrelHostedServiceMethods : IHostedService
{
    private const int Port = 8080;

    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private KestrelServer? _server;

    public KestrelHostedServiceMethods(
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

        await _server.StartAsync(new HttpApplicationMethods(_serviceScopeFactory), cancellationToken);
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

internal class HttpApplicationMethods : IHttpApplication<HttpApplicationContext>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private static DateTime _previousDepositsRequestTime = DateTime.MinValue;
    private static DateTime _previousHealthCheckRequestTime = DateTime.MinValue;

    public HttpApplicationMethods(IServiceScopeFactory serviceScopeFactory)
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

    // TODO: 1. Log requests and responses
    // TODO: 2. Show exceptions as HTML pages if Development environment
    // TODO: 3. Serve static files
    // TODO: 4. Add health check
    // TODO: 5. Add rate limiting
    public async Task ProcessRequestAsync(HttpApplicationContext context)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        await Log(context, scope, async () =>
        {
            await ExceptionPage(context, scope, async () =>
            {
                if (await RouteStaticFiles(context))
                {
                    return;
                }

                if (RouteException(context))
                {
                    return;
                }

                if (await RouteDeposits(context, scope))
                {
                    return;
                }

                if (await RouteHealthCheck(context))
                {
                    return;
                }

                RouteNotFound(context);
            });
        });
    }

    private static async Task Log(HttpApplicationContext context, AsyncServiceScope scope, Func<Task> action)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("RequestResponseLogger");

        logger.LogInformation("Request: {Method} {Path}", requestFeature.Method, requestFeature.Path);
        var stopwatch = Stopwatch.StartNew();

        await action();

        stopwatch.Stop();
        logger.LogInformation("Response: {Method} {Path} HTTP {StatusCode} {ElapsedMilliseconds}ms",
            requestFeature.Method, requestFeature.Path, responseFeature.StatusCode, stopwatch.ElapsedMilliseconds);
    }

    private static async Task ExceptionPage(HttpApplicationContext context, AsyncServiceScope scope, Func<Task> action)
    {
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        var hostEnvironment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            if (hostEnvironment.IsDevelopment())
            {
                var htmlTemplate =
                    @"
<!DOCTYPE html>
<html>
  <head>
    <title>HTTP 500 Internal Server Error</title>
  </head>
  <body>
    <p>Internal Server Error occured. Exception:</p>
    <p>{0}</p>
  </body>
</html>
";
                responseFeature.Headers.Add("Content-Type", new StringValues("text/html; charset=UTF-8"));
                await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(string.Format(htmlTemplate, ex)));
            }
            else
            {
                responseFeature.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }

    private static async Task<bool> RouteStaticFiles(HttpApplicationContext context)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (!requestFeature.Path.StartsWith("/public"))
        {
            return false;
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "public", requestFeature.Path[8..]);

        if (!Path.Exists(filePath))
        {
            return false;
        }

        var extension = Path.GetExtension(filePath);

        responseFeature.StatusCode = StatusCodes.Status200OK;

        if (extension.Equals(".html", StringComparison.InvariantCultureIgnoreCase))
        {
            responseFeature.Headers.Add("Content-Type", new StringValues("text/html; charset=UTF-8"));
        }
        else if (extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
        {
            responseFeature.Headers.Add("Content-Type", new StringValues("image/png"));
        }
        else
        {
            responseFeature.Headers.Add("Content-Type", new StringValues("application/octet-stream"));
        }
        await using var fileStream = File.OpenRead(filePath);
        await fileStream.CopyToAsync(responseBodyFeature.Stream);
        await responseBodyFeature.CompleteAsync();

        return true;
    }

    private static bool RouteException(HttpApplicationContext context)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;

        if (!requestFeature.Path.Equals("/exception", StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        throw new Exception("You hit the exception route");
    }

    private static async Task<bool> RouteDeposits(HttpApplicationContext context, AsyncServiceScope scope)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (!requestFeature.Path.Equals("/deposits", StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        if (DateTime.UtcNow - _previousDepositsRequestTime < TimeSpan.FromSeconds(10))
        {
            responseFeature.StatusCode = StatusCodes.Status429TooManyRequests;
        }
        else
        {
            var depositRepository = scope.ServiceProvider.GetRequiredService<IDepositRepository>();

            var depositModels = (await depositRepository.LoadAllDeposits(CancellationToken.None))
                .Select(x => new DepositDto
                {
                    UserId = x.UserId,
                    Currency = x.Currency,
                    Amount = x.Amount,
                    IsConfirmed = x.IsConfirmed,
                });

            responseFeature.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));

            _previousDepositsRequestTime = DateTime.UtcNow;
        }

        return true;
    }

    private static async Task<bool> RouteHealthCheck(HttpApplicationContext context)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (!requestFeature.Path.Equals("/health", StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        if (DateTime.UtcNow - _previousHealthCheckRequestTime < TimeSpan.FromSeconds(5))
        {
            responseFeature.StatusCode = StatusCodes.Status429TooManyRequests;
        }
        else
        {
            responseFeature.Headers.Add("Content-Type", new StringValues("text/plain; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync("OK"u8.ToArray());

            _previousHealthCheckRequestTime = DateTime.UtcNow;
        }

        return true;
    }

    private static void RouteNotFound(HttpApplicationContext context)
    {
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;

        responseFeature.StatusCode = StatusCodes.Status404NotFound;
    }
}
