using System.Net.Mime;
using System.Text.Json;

namespace ResourcesLab.Features;

public static class MimeTypes
{
    public static WebApplication? UseMimeTypes(this WebApplication app)
    {
        app.MapGet("/mime/not-set", httpContext =>
            httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Hello = "World",
            })));

        app.MapGet("/mime/set", httpContext =>
        {
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            return httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Hello = "World",
            }));
        });

        return app;
    }
}
