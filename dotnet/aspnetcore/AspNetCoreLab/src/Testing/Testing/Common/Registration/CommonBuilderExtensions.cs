using Testing.Common.Passwords;

namespace Testing.Common.Registration;

public static class CommonBuilderExtensions
{
    public static WebApplicationBuilder AddCommon(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<Argon2IdPasswordHasher>();
        builder.Services.Configure<Argon2IdOptions>(builder.Configuration.GetSection("Common:Passwords:Argon2Id"));
        return builder;
    }
}
