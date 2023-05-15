# .NET Dependency Injection

## General

## Some Framework-registered Services

Service Type | Lifetime
-------------|------------
`Microsoft.Extensions.DependencyInjection.IServiceScopeFactory` | Singleton
`IHostApplicationLifetime` | Singleton
`Microsoft.Extensions.Logging.ILogger<TCategoryName>` | Singleton
`Microsoft.Extensions.Logging.ILoggerFactory` | Singleton
`Microsoft.Extensions.ObjectPool.ObjectPoolProvider` | Singleton
`Microsoft.Extensions.Options.IConfigureOptions<TOptions>` | Transient
`Microsoft.Extensions.Options.IOptions<TOptions>` | Singleton
`System.Diagnostics.DiagnosticListener` | Singleton
`System.Diagnostics.DiagnosticSource` | Singleton

## Links

https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection

#dotnet-di
