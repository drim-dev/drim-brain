using Vertical.Features.Users.Options;

namespace Vertical.Features.Deposits.Registration;

public static class UsersServicesExtensions
{
    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<UsersOptions>(builder.Configuration.GetSection("Features:Users"));

        return builder;
    }
}
