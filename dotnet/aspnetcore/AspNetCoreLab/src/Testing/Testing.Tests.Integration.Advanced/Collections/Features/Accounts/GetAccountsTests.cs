using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using Testing.Common.Passwords;
using Testing.Database;
using Testing.Features.Accounts.Domain;
using Testing.Features.Accounts.Requests;
using Testing.Features.Users.Domain;
using Testing.Tests.Integration.Advanced.Collections.Fixtures;
using Testing.Tests.Integration.Advanced.Collections.Helpers;
using Testing.Tests.Integration.Advanced.Containers.Features.Accounts.Contracts;

namespace Testing.Tests.Integration.Advanced.Collections.Features.Accounts;

[Collection(AccountsTestsCollection.Name)]
public class GetAccountsTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    private AsyncServiceScope _scope;

    public GetAccountsTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_get_accounts()
    {
        // Arrange
        var passwordHasher = _scope.ServiceProvider.GetRequiredService<Argon2IdPasswordHasher>();

        var (client, user) = await _fixture.HttpClient.CreateAuthenticatedClient(Create.CancellationToken());

        var anotherUser = new User
        {
            Email = "example@example.com",
            PasswordHash = passwordHasher.HashPassword("qwerty123456A!"),
            DateOfBirth = new DateTime(2001, 01, 31).ToUniversalTime(),
            RegisteredAt = DateTime.UtcNow,
        };

        await _fixture.Database.Execute(async x =>
        {
            x.Users.AddRange(anotherUser);
            await x.SaveChangesAsync();
        });

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

        await _fixture.Database.Execute(async x =>
        {
            x.Accounts.AddRange(account1, account2, account3);
            await x.SaveChangesAsync();
        });

        // Act
        var response = await client.GetFromJsonAsync<GetAccountsResponseContract>("/accounts");

        // Assert
        response.Should().NotBeNull();
        response!.Accounts.Should().NotBeNullOrEmpty();

        var accounts = response.Accounts;
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
        var (client, _) = await _fixture.HttpClient.CreateWronglyAuthenticatedClient(Create.CancellationToken());

        // Act
        var httpResponse = await client.GetAsync("/accounts");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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

[Collection(AccountsTestsCollection.Name)]
public class GetAccountsValidatorTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    private AsyncServiceScope _scope;

    private GetAccounts.RequestValidator? _validator;

    public GetAccountsValidatorTests(TestFixture fixture)
    {
        _fixture = fixture;
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

        await _fixture.Database.Execute(async x =>
        {
            x.Users.Add(user);
            await x.SaveChangesAsync();
        });

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
        await _fixture.Database.Clear(Create.CancellationToken());

        _scope = _fixture.Factory.Services.CreateAsyncScope();

        _validator = new GetAccounts.RequestValidator(_scope.ServiceProvider.GetRequiredService<TestingDbContext>());
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}
