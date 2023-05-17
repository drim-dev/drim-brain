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

public class KestrelHostedService : IHostedService
{
    private const int Port = 8080;

    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHostEnvironment _hostEnvironment;
    private KestrelServer? _server;

    public KestrelHostedService(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        IHostEnvironment hostEnvironment)
    {
        _loggerFactory = loggerFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var transportOptions = new OptionsWrapper<SocketTransportOptions>(new SocketTransportOptions());
        var transportFactory = new SocketTransportFactory(transportOptions, _loggerFactory);

        var serverOptions = new OptionsWrapper<KestrelServerOptions>(new());
        serverOptions.Value.ListenAnyIP(Port);

        _server = new KestrelServer(serverOptions, transportFactory, _loggerFactory);

        await _server.StartAsync(new HttpApplication(_serviceScopeFactory, _hostEnvironment), cancellationToken);
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
    private readonly IHostEnvironment _hostEnvironment;

    private DateTime _previousDepositsRequestTime = DateTime.MinValue;
    private DateTime _previousHealthCheckRequestTime = DateTime.MinValue;

    public HttpApplication(
        IServiceScopeFactory serviceScopeFactory,
        IHostEnvironment hostEnvironment)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _hostEnvironment = hostEnvironment;
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
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("RequestResponseLogger");

        logger.LogInformation("Request: {Method} {Path}", requestFeature.Method, requestFeature.Path);
        var stopwatch = Stopwatch.StartNew();

        bool matched = false;

        try
        {
            if (requestFeature.Path.StartsWith("/public"))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "public", requestFeature.Path[8..]);
                var extension = Path.GetExtension(filePath);

                if (Path.Exists(filePath))
                {
                    matched = true;

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
                }
            }

            if (requestFeature.Path.Equals("/exception", StringComparison.InvariantCultureIgnoreCase))
            {
                matched = true;

                throw new Exception("You hit the exception route");
            }

            if (requestFeature.Path.Equals("/deposits", StringComparison.InvariantCultureIgnoreCase))
            {
                matched = true;

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
            }

            if (requestFeature.Path.Equals("/health", StringComparison.InvariantCultureIgnoreCase))
            {
                matched = true;

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
            }

            if (!matched)
            {
                responseFeature.StatusCode = StatusCodes.Status404NotFound;
            }
        }
        catch (Exception ex)
        {
            if (_hostEnvironment.IsDevelopment())
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

        stopwatch.Stop();
        logger.LogInformation("Response: {Method} {Path} HTTP {StatusCode} {ElapsedMilliseconds}ms",
            requestFeature.Method, requestFeature.Path, responseFeature.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
