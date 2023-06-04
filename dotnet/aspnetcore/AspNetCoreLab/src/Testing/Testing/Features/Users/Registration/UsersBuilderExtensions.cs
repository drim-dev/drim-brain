using Testing.Features.Users.Options;

namespace Testing.Features.Users.Registration;

public static class UsersBuilderExtensions
{
    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<UsersOptions>(builder.Configuration.GetSection("Features:Users"));
        return builder;
    }
}
