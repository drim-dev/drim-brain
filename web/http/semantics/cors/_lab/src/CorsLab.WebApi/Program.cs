using System.Net.Mime;
using System.Text.Json;

const string corsPolicyName = "CorsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policy => policy
        .WithOrigins("http://frontend.drim.city")
        .WithMethods("GET", "PUT")
        .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors(corsPolicyName);

app.MapGet("/", httpContext =>
{
    var headers = httpContext.Request.Headers
        .ToDictionary(header => header.Key, header => header.Value);

    httpContext.Response.ContentType = MediaTypeNames.Application.Json;

    foreach (var header in headers)
    {
        httpContext.Response.Headers[$"{header.Key}"] = header.Value;
    }

    return httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
    {
        RequestHeaders = headers,
        RemoteIp = httpContext.Connection.RemoteIpAddress?.ToString(),
        Scheme = httpContext.Request.Scheme,
        Host = httpContext.Request.Host.Value,
    }));
});

app.MapPut("/", () => JsonSerializer.Serialize(new { Data = "PUT Data" }));

app.Run();
