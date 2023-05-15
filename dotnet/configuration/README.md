# .NET Configuration

## General

Configuration in .NET is performed using one or more configuration providers. Configuration providers read configuration data from key-value pairs using various configuration sources:

* Settings files, such as appsettings.json
* Environment variables
* Azure Key Vault
* Azure App Configuration
* Command-line arguments
* Custom providers, installed or created
* Directory files
* In-memory .NET objects
* Third-party providers

Given one or more configuration sources, the `IConfiguration` type provides a unified view of the configuration data. Configuration is read-only, and the configuration pattern isn't designed to be programmatically writable. The `IConfiguration` interface is a single representation of all the configuration sources.

## Options Pattern

The options pattern uses classes to provide strongly-typed access to groups of related settings. When configuration settings are isolated by scenario into separate classes, the app adheres to two important software engineering principles:

* The Interface Segregation Principle (ISP) or Encapsulation: Scenarios (classes) that depend on configuration settings depend only on the configuration settings that they use.
* Separation of Concerns: Settings for different parts of the app aren't dependent or coupled with one another.

Options also provide a mechanism to validate configuration data.

https://learn.microsoft.com/en-us/dotnet/core/extensions/options

```json
{
  "TransientFaultHandlingOptions": {
    "Enabled": true,
    "AutoRetryDelay": "00:00:07"
  }
}
```

```csharp
public sealed class TransientFaultHandlingOptions
{
    public bool Enabled { get; set; }
    public TimeSpan AutoRetryDelay { get; set; }
}
```

```csharp
Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<TransientFaultHandlingOptions>(
            context.Configuration.GetSection(nameof(TransientFaultHandlingOptions)));
    });
```

### `IOptions<TOptions>`

// TODO

### `IOptionsSnapshot<TOptions>`

// TODO

### `IOptionsMonitor<TOptions>`

// TODO

### `IOptionsFactory<TOptions>`

// TODO

### `IOptionsMonitorCache<TOptions>`

// TODO

### `IOptionsChangeTokenSource<TOptions>`

// TODO

## Links

https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration

#dotnet-configuration
