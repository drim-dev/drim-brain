using System.Data;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testing.Tests.Integration.Advanced.Collections.Harnesses.Base;

namespace Testing.Tests.Integration.Advanced.Collections.Harnesses;

public class DatabaseHarness<TProgram, TDbContext> : IHarness<TProgram>
    where TProgram : class
    where TDbContext : DbContext
{
    private const string Username = "db_creator";
    private const string Password = "12345678";

    private IContainer? _postgres;
    private WebApplicationFactory<TProgram>? _factory;
    private bool _started;
    private int _port;

    public void ConfigureWebHostBuilder(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {
                    "ConnectionStrings:TestingDbContext",
                    $"Host=localhost;Port={_port};Database=Tests;" +
                    $"Username={Username};Password={Password};" +
                    "Maximum Pool Size=10;Connection Idle Lifetime=60;"
                },
            });
        });
    }

    public async Task Start(WebApplicationFactory<TProgram> factory, CancellationToken cancellationToken)
    {
        _factory = factory;

        _postgres = new ContainerBuilder()
            .WithImage("postgres:15.2")
            .WithPortBinding(5432, true)
            .WithEnvironment("POSTGRES_USER", Username)
            .WithEnvironment("POSTGRES_PASSWORD", Password)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();

        await _postgres.StartAsync(cancellationToken);

        _port = _postgres.GetMappedPublicPort(5432);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await db.Database.MigrateAsync(cancellationToken: cancellationToken);

        _started = true;
    }

    public async Task Stop(CancellationToken cancellationToken)
    {
        if (_postgres is not null)
        {
            await _postgres.StopAsync(cancellationToken);
            await _postgres.DisposeAsync();

            await Task.Delay(2_000, cancellationToken); // TODO: use health check
        }

        _started = false;
    }

    public async Task Execute(Func<TDbContext, Task> action)
    {
        ThrowIfNotStarted();

        await using var scope = _factory!.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await action(db);
    }

    public async Task<T> Execute<T>(Func<TDbContext, Task<T>> action)
    {
        if (!_started)
        {
            throw new InvalidOperationException($"Database harness is not started. Call {nameof(Start)} first.");
        }

        await using var scope = _factory!.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        return await action(db);
    }

    public async Task Clear(CancellationToken cancellationToken)
    {
        await using var scope = _factory!.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await using var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            SchemasToInclude = new [] { "public" },
            DbAdapter = DbAdapter.Postgres,
        });

        await respawner.ResetAsync(connection);
    }

    private void ThrowIfNotStarted()
    {
        if (!_started)
        {
            throw new InvalidOperationException($"Database harness is not started. Call {nameof(Start)} first.");
        }
    }
}
