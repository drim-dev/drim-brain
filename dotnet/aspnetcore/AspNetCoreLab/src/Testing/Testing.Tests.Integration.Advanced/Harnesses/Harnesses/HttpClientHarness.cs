using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Testing.Database;
using Testing.Features.Users.Domain;
using Testing.Tests.Integration.Advanced.Harnesses.Harnesses.Base;

namespace Testing.Tests.Integration.Advanced.Harnesses.Harnesses;

public class HttpClientHarness<TProgram> : IHarness<TProgram>
    where TProgram : class
{
    private readonly DatabaseHarness<TProgram, TestingDbContext> _databaseHarness;
    private WebApplicationFactory<TProgram>? _factory;
    private bool _started;

    public HttpClientHarness(DatabaseHarness<TProgram, TestingDbContext> databaseHarness)
    {
        _databaseHarness = databaseHarness;
    }

    public void ConfigureWebHostBuilder(IWebHostBuilder builder)
    {
    }

    public Task Start(WebApplicationFactory<TProgram> factory, CancellationToken cancellationToken)
    {
        _factory = factory;
        _started = true;

        return Task.CompletedTask;
    }

    public Task Stop(CancellationToken cancellationToken)
    {
        _started = false;

        return Task.CompletedTask;
    }

    public HttpClient CreateClient()
    {
        ThrowIfNotStarted();

        return _factory!.CreateClient();
    }

    public async Task<(HttpClient, User user)> CreateAuthenticatedClient(CancellationToken cancellationToken)
    {
        ThrowIfNotStarted();

        var user = new User
        {
            Email = $"{Guid.NewGuid()}@test.com",
            PasswordHash = Guid.NewGuid().ToString(),
            DateOfBirth = DateTime.UtcNow,
            RegisteredAt = DateTime.UtcNow,
        };
        await _databaseHarness.Execute(async context =>
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);
        });

        // TODO: use config
        var key = "ogbpxta0VgQWXsBsFeeIiNjF4nhK17ewp2SVkASR"u8.ToArray();
        var token = new JwtSecurityToken(
            issuer: "crypto-bank",
            audience: "crypto-bank",
            claims: new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            },
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var client = _factory!.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        return (client, user);
    }

    public async Task<(HttpClient, User user)> CreateWronglyAuthenticatedClient(CancellationToken cancellationToken)
    {
        ThrowIfNotStarted();

        var user = new User
        {
            Email = $"{Guid.NewGuid()}@test.com",
            PasswordHash = Guid.NewGuid().ToString(),
            DateOfBirth = DateTime.UtcNow,
            RegisteredAt = DateTime.UtcNow,
        };
        await _databaseHarness.Execute(async context =>
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);
        });

        // TODO: use config
        var key = "invalidKeyinvalidKeyinvalidKeyinvalidKey"u8.ToArray();
        var token = new JwtSecurityToken(
            issuer: "crypto-bank",
            audience: "crypto-bank",
            claims: new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user!.Id.ToString()),
            },
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var client = _factory!.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        return (client, user);
    }

    private void ThrowIfNotStarted()
    {
        if (!_started)
        {
            throw new InvalidOperationException($"HTTP client harness is not started. Call {nameof(Start)} first.");
        }
    }
}
