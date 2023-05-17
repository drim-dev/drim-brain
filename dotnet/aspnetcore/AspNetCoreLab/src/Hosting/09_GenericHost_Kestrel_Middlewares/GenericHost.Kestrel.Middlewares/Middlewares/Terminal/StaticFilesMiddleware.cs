using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace GenericHost.Kestrel.Middlewares.Middlewares.Terminal;

public class StaticFilesMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (!requestFeature.Path.StartsWith("/public"))
        {
            await next();
            return;
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "public", requestFeature.Path[8..]);

        if (!Path.Exists(filePath))
        {
            await next();
            return;
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
    }
}
