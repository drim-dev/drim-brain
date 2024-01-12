using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse("10.0.1.1"));
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;
});

var app = builder.Build();

app.UseForwardedHeaders();

app.MapGet("/", httpContext =>
{
    var headers = httpContext.Request.Headers
        .ToDictionary(header => header.Key, header => header.Value);

    httpContext.Response.ContentType = MediaTypeNames.Application.Json;

    foreach (var header in headers)
    {
        httpContext.Response.Headers[$"Request-{header.Key}"] = header.Value;
    }

    return httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
    {
        RequestHeaders = headers,
        RemoteIp = httpContext.Connection.RemoteIpAddress?.ToString(),
        Scheme = httpContext.Request.Scheme,
        Host = httpContext.Request.Host.Value,
        DnsHostName = Dns.GetHostName(),
    }));
});

app.MapPut("/", () => JsonSerializer.Serialize(new { Data = "PUT Data" }));

app.Run();
