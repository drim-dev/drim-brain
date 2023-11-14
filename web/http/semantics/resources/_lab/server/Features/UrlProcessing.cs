using System.Text.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ResourcesLab.Features;

public static class UrlProcessing
{
    public static WebApplication? UseUrlProcessing(this WebApplication app)
    {
        app.MapGet("/urls/{*_}", httpContext =>
            httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Scheme = httpContext.Request.Scheme,
                Authority = httpContext.Request.Host.Host,
                Port = httpContext.Request.Host.Port,
                Path = httpContext.Request.Path.Value,
                Query = httpContext.Request.QueryString.Value,
                Fragment = "Never sent to the server",
                Url = httpContext.Request.GetDisplayUrl(),
            })));

        app.MapGet("/urls/bind/{*route}", (
                [FromRoute] string route,
                [FromQuery] string parameter1,
                [FromQuery] string parameter2,
                HttpContext httpContext) =>
            httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Path = route,
                Query = new
                {
                    parameter1 = parameter1,
                    parameter2 = parameter2,
                },
            })));

        app.MapGet("/urls/encode", (
            [FromQuery] string value,
            HttpContext httpContext) =>
            httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Value = value,
                Encoded = Uri.EscapeDataString(value),
            })));

        return app;
    }
}
