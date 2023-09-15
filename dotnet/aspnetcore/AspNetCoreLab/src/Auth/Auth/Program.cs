using System.Reflection;
using System.Security.Claims;
using System.Text;
using Auth.Authorization;
using Auth.Authorization.Requirements;
using Auth.Features.Auth.Domain;
using Auth.Features.Auth.Options;
using Auth.Pipeline.Behaviors;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    var jwtOptions = builder.Configuration.GetSection("Features:Auth").Get<AuthOptions>()!.Jwt;

    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
        RoleClaimType = "role",
    };
});

builder.Services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SellerRankRequirementHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.BuyerRole, policy => policy.AddRequirements(new RoleRequirement(UserRole.Buyer)));
    options.AddPolicy(PolicyNames.SellerRole, policy => policy.AddRequirements(new RoleRequirement(UserRole.Seller)));
    options.AddPolicy(PolicyNames.OwnerRole, policy => policy.RequireClaim(ClaimTypes.Role, UserRole.Owner.ToString()));

    options.AddPolicy(PolicyNames.SellerFromRank3, policy => policy.AddRequirements(new SellerRankRequirement(3), new RoleRequirement(UserRole.Owner)));
    options.AddPolicy(PolicyNames.OpenShift, policy => policy.AddRequirements(new ActionRequirement("OpenShift")));
});

builder.Services.AddControllers();

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Features:Auth"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
