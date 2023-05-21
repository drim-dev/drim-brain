using Vertical.Features.Auth.Options;

namespace Vertical.Features.Auth.Registration;

public static class AuthBuilderExtensions
{
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Features:Auth"));

        return builder;
    }
}
