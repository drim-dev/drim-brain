# .NET Hosting

## General

A _host_ is an object that encapsulates an app's resources and lifetime functionality, such as:

* Dependency injection (DI)
* Logging
* Configuration
* App shutdown
* `IHostedService` implementations

Simple host creation:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
```

Alternative streamlined console-only API:

```csharp
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();
```

The following defaults are applied to the `HostBuilder`:

* Set the `ContentRootPath` to the result of `GetCurrentDirectory()`.
* Load host `IConfiguration` from "DOTNET_" prefixed environment variables.
* Load host `IConfiguration` from supplied command line arguments.
* Load app `IConfiguration` from 'appsettings.json' and 'appsettings.[EnvironmentName].json'.
* Load app `IConfiguration` from User Secrets when `EnvironmentName` is 'Development' using the entry assembly.
* Load app `IConfiguration` from environment variables.
* Load app `IConfiguration` from supplied command line arguments.
* Configure the `ILoggerFactory` to log to the console, debug, and event source output.
* Enable scope validation on the dependency injection container when `EnvironmentName` is 'Development'.

## Links

https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host

#dotnet-hosting
