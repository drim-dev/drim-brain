using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Testing.Common.Passwords;
using Testing.Database;
using Testing.Features.Accounts.Domain;
using Testing.Features.Accounts.Requests;
using Testing.Features.Auth.Options;
using Testing.Features.Users.Domain;
using Testing.Tests.Integration.Advanced.Containers.Features.Accounts.Contracts;

namespace Testing.Tests.Integration.Advanced.Containers.Features.Accounts;

public class GetAccountsTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    private IContainer? _postgres;
    private TestingDbContext? _db;
    private AsyncServiceScope _scope;

    public GetAccountsTests()
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
    public async Task Should_get_accounts()
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

        var anotherUser = new User
        {
            Email = "example@example.com",
            PasswordHash = passwordHasher.HashPassword("qwerty123456A!"),
            DateOfBirth = new DateTime(2001, 01, 31).ToUniversalTime(),
            RegisteredAt = DateTime.UtcNow,
        };

        _db!.Users.AddRange(user, anotherUser);
        await _db.SaveChangesAsync();

        var account1 = new Account
        {
            Number = "ACC1",
            Currency = "BTC",
            Amount = 100,
            UserId = user.Id,
        };

        var account2 = new Account
        {
            Number = "ACC2",
            Currency = "BTC",
            Amount = 200,
            UserId = user.Id,
        };

        var account3 = new Account
        {
            Number = "ACC3",
            Currency = "BTC",
            Amount = 300,
            UserId = anotherUser.Id,
        };

        _db.Accounts.AddRange(account1, account2, account3);
        await _db.SaveChangesAsync();

        var jwtOptions = _scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>().Value.Jwt;
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()), // TODO: check
            },
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha512Signature)
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        // Act
        var httpResponse = (await client.GetAsync("/accounts"))
            .EnsureSuccessStatusCode();

        // Assert
        httpResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        var content = await httpResponse.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<GetAccountsResponseContract>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        contract.Should().NotBeNull();
        contract!.Accounts.Should().NotBeNullOrEmpty();

        var accounts = contract.Accounts;
        accounts.Should().HaveCount(2);

        var account1Contract = accounts.Single(a => a.Number == account1.Number);
        account1Contract.Currency.Should().Be(account1.Currency);
        account1Contract.Amount.Should().Be(account1.Amount);

        var account2Contract = accounts.Single(a => a.Number == account2.Number);
        account2Contract.Currency.Should().Be(account2.Currency);
        account2Contract.Amount.Should().Be(account2.Amount);
    }

    [Fact]
    public async Task Should_not_authenticate_if_wrong_jwt()
    {
        var jwtOptions = _scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>().Value.Jwt;
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
            },
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey("wrong_key_1234567890"u8.ToArray()), SecurityAlgorithms.HmacSha512Signature)
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        // Act
        var httpResponse = await client.GetAsync("/accounts");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public async Task InitializeAsync()
    {
        _postgres = new ContainerBuilder()
            .WithImage("postgres:15.2")
            .WithPortBinding(15432, 5432)
            .WithEnvironment("POSTGRES_USER", "db_creator")
            .WithEnvironment("POSTGRES_PASSWORD", "12345678")
            .Build();
        await _postgres.StartAsync();

        await Task.Delay(2_000); // TODO: use health check

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

public class GetAccountsValidatorTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    private IContainer? _postgres;
    private TestingDbContext? _db;
    private AsyncServiceScope _scope;

    private GetAccounts.RequestValidator? _validator;

    public GetAccountsValidatorTests()
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
    public async Task Should_validate_correct_request()
    {
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = "123",
            DateOfBirth = new DateTime(2000, 01, 31).ToUniversalTime(),
            RegisteredAt = DateTime.UtcNow,
        };

        _db!.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _validator.TestValidateAsync(new GetAccounts.Request(user.Id));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_validate_user_not_found()
    {
        var result = await _validator.TestValidateAsync(new GetAccounts.Request(1));
        result.ShouldHaveValidationErrorFor(x => x.UserId).WithErrorCode("accounts_validation_user_not_found");
    }

    public async Task InitializeAsync()
    {
        _postgres = new ContainerBuilder()
            .WithImage("postgres:15.2")
            .WithPortBinding(15432, 5432)
            .WithEnvironment("POSTGRES_USER", "db_creator")
            .WithEnvironment("POSTGRES_PASSWORD", "12345678")
            .Build();
        await _postgres.StartAsync();

        await Task.Delay(2_000); // TODO: use health check

        var _ = _factory.Server;
        _scope = _factory.Services.CreateAsyncScope();
        _db = _scope.ServiceProvider.GetRequiredService<TestingDbContext>();
        await _db.Database.MigrateAsync();

        _validator = new GetAccounts.RequestValidator(_db);
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
