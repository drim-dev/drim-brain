using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Testing.Common.Passwords;
using Testing.Features.Auth.Options;
using Testing.Features.Auth.Requests;
using Testing.Features.Users.Domain;
using Testing.Tests.Integration.Advanced.Collections.Errors.Contracts;
using Testing.Tests.Integration.Advanced.Collections.Features.Auth.Contracts;
using Testing.Tests.Integration.Advanced.Collections.Fixtures;
using Testing.Tests.Integration.Advanced.Collections.Helpers;

namespace Testing.Tests.Integration.Advanced.Collections.Features.Auth;

[Collection(AuthTestsCollection.Name)]
public class AuthenticateTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    private AsyncServiceScope _scope;

    public AuthenticateTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_authenticate_user()
    {
        // Arrange
        var passwordHasher = _scope.ServiceProvider.GetRequiredService<Argon2IdPasswordHasher>();

        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = passwordHasher.HashPassword("qwerty123456A!"),
            DateOfBirth = new DateTime(2000, 01, 31).ToUniversalTime(),
            RegisteredAt = DateTime.UtcNow,
        };

        await _fixture.Database.Execute(async x =>
        {
            x.Users.Add(user);
            await x.SaveChangesAsync();
        });

        var client = _fixture.HttpClient.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync<AccessTokenContract>("/auth/authenticate", new
            {
                Email = "test@test.com",
                Password = "qwerty123456A!",
            });

        response.Should().NotBeNull();
        response.Jwt.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtOptions = _scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>().Value.Jwt;
        var key = jwtOptions.SigningKey;
        tokenHandler.ValidateToken(response.Jwt, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        validatedToken.Should().NotBeNull();
        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = long.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        userId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Should_return_validation_error_if_user_not_found_by_email()
    {
        // Arrange

        // No user in DB

        var client = _fixture.HttpClient.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync<ValidationProblemDetailsContract>("/auth/authenticate", new
        {
            Email = "test@test.com",
            Password = "qwerty123456A!",
        }, HttpStatusCode.BadRequest);

        // Assert
        response.ShouldContain(string.Empty, "Wrong email or password", "auth_validation_wrong_credentials");
    }

    [Fact]
    public async Task Should_return_validation_error_if_password_is_incorrect()
    {
        // Arrange
        var passwordHasher = _scope.ServiceProvider.GetRequiredService<Argon2IdPasswordHasher>();

        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = passwordHasher.HashPassword("wrong_password"),
            DateOfBirth = new DateTime(2000, 01, 31).ToUniversalTime(),
            RegisteredAt = DateTime.UtcNow,
        };

        await _fixture.Database.Execute(async x =>
        {
            x.Users.Add(user);
            await x.SaveChangesAsync();
        });

        var client = _fixture.HttpClient.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync<ValidationProblemDetailsContract>("/auth/authenticate", new
        {
            Email = "test@test.com",
            Password = "qwerty123456A!",
        }, HttpStatusCode.BadRequest);

        // Assert
        response.ShouldContain(string.Empty, "Wrong email or password", "auth_validation_wrong_credentials");
    }

    public async Task InitializeAsync()
    {
        await _fixture.Database.Clear(Create.CancellationToken());

        _scope = _fixture.Factory.Services.CreateAsyncScope();
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}

[Collection(AuthTestsCollection.Name)]
public class AuthenticateValidatorTests
{
    private readonly Authenticate.RequestValidator _validator = new();

    [Fact]
    public void Should_validate_correct_request()
    {
        var result = _validator.TestValidate(new Authenticate.Request("test@test.com", "password"));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_require_email(string email)
    {
        var result = _validator.TestValidate(new Authenticate.Request(email, "password"));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("auth_validation_email_required");
    }

    [Theory]
    [InlineData("test")]
    [InlineData("@test.com")]
    [InlineData("test@")]
    public void Should_validate_email_format(string email)
    {
        var result = _validator.TestValidate(new Authenticate.Request(email, "password"));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("auth_validation_invalid_email_format");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_require_password(string password)
    {
        var result = _validator.TestValidate(new Authenticate.Request("test@test.com", password));
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorCode("auth_validation_password_required");
    }
}
