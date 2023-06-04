using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testing.Common.Passwords;
using Testing.Database;

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
