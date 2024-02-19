# Kestrel Connection Management

## General Limits

* __KeepAliveTimeout__. Gets or sets the keep-alive timeout.

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
});
```

* __MaxConcurrentConnections__. Gets or sets the maximum number of open connections.

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxConcurrentConnections = 100;
});
```

* __MaxConcurrentUpgradedConnections__. Gets or sets the maximum number of open, upgraded connections.

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
});
```

* __MaxRequestBodySize__. Gets or sets the maximum allowed size of any request body in bytes.

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100_000_000;
});
```

* __MinRequestBodyDataRate__. Gets or sets the request body minimum data rate in bytes/second. __MinResponseDataRate__. Gets or sets the response minimum data rate in bytes/second.

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(
        bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
    serverOptions.Limits.MinResponseDataRate = new MinDataRate(
        bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
});
```

* __RequestHeadersTimeout__. Gets or sets the maximum amount of time the server spends receiving request headers.

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
});
```

## Links

* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-8.0

#kestrel-connection-management
