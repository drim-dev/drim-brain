using System.Net.Http.Json;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testing.Common.Passwords;
using Testing.Database;
using Testing.Features.Users.Domain;
using Testing.Features.Users.Options;
using Testing.Features.Users.Requests;

namespace Testing.Tests.Integration.Features.Users.Requests;

public class RegisterTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    private TestingDbContext _db;
    private AsyncServiceScope _scope;

    public RegisterTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {
                        "ConnectionStrings:TestingDbContext",
                        "Host=localhost;Database=Testing.Tests;Username=db_creator;Password=12345678;Maximum Pool Size=10;Connection Idle Lifetime=60;"
                    },
                });
            }));
    }

    [Fact]
    public async Task Should_register_user()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        (await client.PostAsJsonAsync("/users", new
            {
                Email = "test@test.com",
                Password = "qwerty123456A!",
                DateOfBirth = "2000-01-31",
            }))
            .EnsureSuccessStatusCode();

        // Assert
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == "test@test.com");
        user.Should().NotBeNull();
        user!.RegisteredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        user.DateOfBirth.Should().Be(new DateTime(2000, 01, 31).ToUniversalTime());

        var passwordHasher = _scope.ServiceProvider.GetRequiredService<Argon2IdPasswordHasher>();
        passwordHasher.VerifyHashedPassword(user.PasswordHash, "qwerty123456A!").Should().BeTrue();
    }

    public Task InitializeAsync()
    {
        var _ = _factory.Server;
        _scope = _factory.Services.CreateAsyncScope();
        _db = _scope.ServiceProvider.GetRequiredService<TestingDbContext>();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _db.Users.RemoveRange(_db.Users);
        await _db.SaveChangesAsync();
        await _scope.DisposeAsync();
    }
}

public class RegisterValidatorTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly UsersOptions _options = new();

    private TestingDbContext _db;
    private AsyncServiceScope _scope;

    private Register.RequestValidator _validator;

    public RegisterValidatorTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {
                        "ConnectionStrings:TestingDbContext",
                        "Host=localhost;Database=Testing.Tests;Username=db_creator;Password=12345678;Maximum Pool Size=10;Connection Idle Lifetime=60;"
                    },
                });
            }));
    }

    [Fact]
    public async Task Should_validate_correct_request()
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", "password", new DateTime(2000, 01, 31).ToUniversalTime()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_require_email(string email)
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request(email, "password", new DateTime(2000, 01, 31).ToUniversalTime()));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("users_validation_email_required");
    }

    [Theory]
    [InlineData("test")]
    [InlineData("@test.com")]
    [InlineData("test@")]
    public async Task Should_validate_email_format(string email)
    {
        var result = await _validator.TestValidateAsync(new
            Register.Request(email, "password", new DateTime(2000, 01, 31).ToUniversalTime()));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("users_validation_invalid_email_format");
    }

    [Fact]
    public async Task Should_validate_email_taken()
    {
        const string email = "test@test.com";

        var existingUser = new User
        {
            Email = email,
            PasswordHash = "123",
            RegisteredAt = DateTime.UtcNow,
            DateOfBirth = new DateTime(2000, 01, 31).ToUniversalTime(),
        };

        _db.Users.Add(existingUser);
        await _db.SaveChangesAsync();

        var result = await _validator.TestValidateAsync(new
            Register.Request(email, "password", new DateTime(2000, 01, 31).ToUniversalTime()));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("users_validation_email_already_taken");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_require_password(string password)
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", password, new DateTime(2000, 01, 31).ToUniversalTime()));
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorCode("users_validation_password_required");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("1234567")]
    public async Task Should_validate_password_length(string password)
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", password, new DateTime(2000, 01, 31).ToUniversalTime()));
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorCode("users_validation_invalid_password_length");
    }

    [Fact]
    public async Task Should_validate_too_old()
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", "12345678", DateTime.UtcNow.AddYears(-1 * (_options.MaximumUserAgeInYears + 1))));
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth).WithErrorCode("users_validation_too_old");
    }

    [Fact]
    public async Task Should_validate_too_young()
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", "12345678", DateTime.UtcNow.AddYears(-1 * (_options.MinimumUserAgeInYears - 1))));
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth).WithErrorCode("users_validation_too_young");
    }

    public Task InitializeAsync()
    {
        var _ = _factory.Server;
        _scope = _factory.Services.CreateAsyncScope();
        _db = _scope.ServiceProvider.GetRequiredService<TestingDbContext>();
        _validator = new Register.RequestValidator(_db, new OptionsWrapper<UsersOptions>(_options));

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _db.Users.RemoveRange(_db.Users);
        await _db.SaveChangesAsync();
        await _scope.DisposeAsync();
    }
}