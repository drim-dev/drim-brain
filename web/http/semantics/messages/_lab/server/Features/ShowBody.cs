using System.Net.Mime;
using System.Text.Json;

namespace MessagesLab.Features;

public static class ShowBody
{
    public static WebApplication? UseShowBody(this WebApplication app)
    {
        app.MapPost("/body", async httpContext =>
        {
            using StreamReader reader = new(httpContext.Request.Body, leaveOpen: false);
            var body = await reader.ReadToEndAsync();

            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                RequestBody = body,
            }));
        });

        return app;
    }
}
