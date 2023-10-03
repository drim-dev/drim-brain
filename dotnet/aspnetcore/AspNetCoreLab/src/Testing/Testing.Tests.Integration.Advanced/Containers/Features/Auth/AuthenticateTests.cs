using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Testing.Common.Passwords;
using Testing.Database;
using Testing.Features.Auth.Options;
using Testing.Features.Auth.Requests;
using Testing.Features.Users.Domain;
using Testing.Tests.Integration.Advanced.Containers.Errors.Contracts;
using Testing.Tests.Integration.Advanced.Containers.Features.Auth.Contracts;

namespace Testing.Tests.Integration.Advanced.Containers.Features.Auth;

public class AuthenticateTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    private IContainer? _postgres;
    private TestingDbContext? _db;
    private AsyncServiceScope _scope;

    public AuthenticateTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {
                        "ConnectionStrings:TestingDbContext",
                        "Host=localhost;Port=15432;Database=Testing.Tests;Username=db_creator;Password=12345678;Maximum Pool Size=10;Connection Idle Lifetime=60;"
                    },
                });
            }));
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

        _db!.Users.Add(user);
        await _db.SaveChangesAsync();

        var client = _factory.CreateClient();

        // Act
        var httpResponse = (await client.PostAsJsonAsync("/auth/authenticate", new
            {
                Email = "test@test.com",
                Password = "qwerty123456A!",
            }))
            .EnsureSuccessStatusCode();

        // Assert
        httpResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        var content = await httpResponse.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<AccessTokenContract>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        contract.Should().NotBeNull();
        contract!.Jwt.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtOptions = _scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>().Value.Jwt;
        var key = jwtOptions.SigningKey;
        tokenHandler.ValidateToken(contract.Jwt, new TokenValidationParameters
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

        var client = _factory.CreateClient();

        // Act
        var httpResponse = await client.PostAsJsonAsync("/auth/authenticate", new
        {
            Email = "test@test.com",
            Password = "qwerty123456A!",
        });

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        httpResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
        var content = await httpResponse.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<ValidationProblemDetailsContract>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        contract.Should().NotBeNull();
        contract!.Title.Should().Be("Validation failed");
        contract.Type.Should().Be("https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400");
        contract.Detail.Should().Be("One or more validation errors have occurred");
        contract.Status.Should().Be(StatusCodes.Status400BadRequest);
        contract.Errors.Should().HaveCount(1);

        var error = contract.Errors.Single();
        error.Field.Should().Be(string.Empty);
        error.Message.Should().Be("Wrong email or password");
        error.Code.Should().Be("auth_validation_wrong_credentials");
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

        _db!.Users.Add(user);
        await _db.SaveChangesAsync();

        var client = _factory.CreateClient();

        // Act
        var httpResponse = await client.PostAsJsonAsync("/auth/authenticate", new
        {
            Email = "test@test.com",
            Password = "qwerty123456A!",
        });

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        httpResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
        var content = await httpResponse.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<ValidationProblemDetailsContract>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        contract.Should().NotBeNull();
        contract!.Title.Should().Be("Validation failed");
        contract.Type.Should().Be("https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400");
        contract.Detail.Should().Be("One or more validation errors have occurred");
        contract.Status.Should().Be(StatusCodes.Status400BadRequest);
        contract.Errors.Should().HaveCount(1);

        var error = contract.Errors.Single();
        error.Field.Should().Be(string.Empty);
        error.Message.Should().Be("Wrong email or password");
        error.Code.Should().Be("auth_validation_wrong_credentials");
    }

    public async Task InitializeAsync()
    {
        _postgres = new ContainerBuilder()
            .WithImage("postgres:15.2")
            .WithPortBinding(15432, 5432)
            .WithEnvironment("POSTGRES_USER", "db_creator")
            .WithEnvironment("POSTGRES_PASSWORD", "12345678")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();
        await _postgres.StartAsync();

        var _ = _factory.Server;
        _scope = _factory.Services.CreateAsyncScope();
        _db = _scope.ServiceProvider.GetRequiredService<TestingDbContext>();
        await _db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        _db!.Users.RemoveRange(_db.Users);
        await _db.SaveChangesAsync();
        await _scope.DisposeAsync();

        if (_postgres is not null)
        {
            await _postgres.StopAsync();
            await _postgres.DisposeAsync();
        }
    }
}

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
