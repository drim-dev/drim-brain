using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace GenericHost.Kestrel.Endpoints.Middlewares;

public class ExceptionPageMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        var hostEnvironment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        try
        {
            await next();
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
}
