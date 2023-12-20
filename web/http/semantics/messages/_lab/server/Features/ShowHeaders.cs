using System.Net.Mime;
using System.Text.Json;

namespace MessagesLab.Features;

public static class ShowHeaders
{
    public static WebApplication? UseShowHeaders(this WebApplication app)
    {
        app.MapGet("/headers", httpContext =>
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
            }));
        });

        return app;
    }
}
