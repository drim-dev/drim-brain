using Vertical.Features.Auth.Options;

namespace Vertical.Features.Deposits.Registration;

public static class AuthServicesExtensions
{
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Features:Auth"));

        return builder;
    }
}
