using Microsoft.Extensions.FileProviders;

namespace ResourcesLab.Features;

public static class StaticFiles
{
    public static WebApplication? UseStaticFiles(this WebApplication app)
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "static")),
            RequestPath = "/static",
        });

        return app;
    }
}
