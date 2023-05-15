# .NET Logging

## General

## Configuration

Logging configuration is commonly provided by the `Logging` section of appsettings.`{Environment}`.json files.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "Debug": {
      "LogLevel": {
        "Default": "Information"
      }
    },
    "Console": {
      "IncludeScopes": true,
      "LogLevel": {
        "Microsoft.Extensions.Hosting": "Warning",
        "Default": "Information"
      }
    },
    "EventSource": {
      "LogLevel": {
        "Microsoft": "Information"
      }
    }
  }
}
```

## Non-Host Console App

```csharp
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Example log message");
```

## Links

https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line

#dotnet-logging
