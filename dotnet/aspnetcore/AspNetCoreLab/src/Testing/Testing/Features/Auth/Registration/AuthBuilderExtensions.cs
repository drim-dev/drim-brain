using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Testing.Features.Auth.Options;

namespace Testing.Features.Auth.Registration;

public static class AuthBuilderExtensions
{
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        const string sectionName = "Features:Auth";

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
            builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(sectionName));

            var jwtOptions = builder.Configuration.GetSection(sectionName).Get<AuthOptions>()!.Jwt;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
            };
        });

        return builder;
    }
}
