using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace ResourcesLab.Features;

public static class StaticFiles
{
    public static WebApplication? UseStaticFiles(this WebApplication app)
    {
        var contentTypeProvider = new FileExtensionContentTypeProvider
        {
            Mappings =
            {
                [".image2"] = MediaTypeNames.Image.Png,
                [".image3"] = MediaTypeNames.Application.Octet,
            }
        };

        app.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = "/static",
            FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "static")),
            ContentTypeProvider = contentTypeProvider,

            // WARNING! This is a security risk. Do not use in production.
            ServeUnknownFileTypes = true,
        });

        return app;
    }
}
