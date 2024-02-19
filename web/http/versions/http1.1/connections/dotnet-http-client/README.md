# .NET HttpClient Connection Management

The `System.Net.Http.HttpClient` class sends HTTP requests and receives HTTP responses from a resource identified by a URI. An `HttpClient` instance is a collection of settings that's applied to all requests executed by that instance, and each instance uses its own connection pool, which isolates its requests from others. Starting in .NET Core 2.1, the `SocketsHttpHandler` class provides the implementation, making behavior consistent across all platforms.

## `SocketsHttpHandler`

`SocketsHttpHandler` was built directly on top of the Socket APIs, implementing HTTP in managed code. It has following advantages:

* A significant performance improvement when compared with the previous implementation.
* The elimination of platform dependencies, which simplifies deployment and servicing.
* Consistent behavior across all .NET platforms.

`SocketsHttpHandler` class has a connection pooling mechanism to manage opened connections per unique endpoint. Basically, after sending an HTTP request, the `SocketsHttpHandler` class will add the TCP connection to the pool in order to reuse it in the future for subsequent requests to the same endpoint.

Here are the properties of the `SocketsHttpHandler` that you need to be aware of to manage the connection pool:

* __MaxConnectionPerServer__. The maximum number of TCP connections allowed per single endpoint. The default value is equal to `Int32.MaxValue`.
* __PooledConnectionIdleTimeout__. Determines how long a TCP connection can stay unused in the pool. After this timeout, the `SocketsHttpHandler` object will remove the connection from the pool.
* __PooledConnectionLifetime__. The amount of time an active TCP connection can remain in the pool. The word “active” refers to a connection that is currently being used.

## DNS Behavior

`HttpClient` only resolves DNS entries when a connection is created. It does not track any time to live (TTL) durations specified by the DNS server. If DNS entries change regularly, which can happen in some scenarios, the client won't respect those updates. To solve this issue, you can limit the lifetime of the connection by setting the `PooledConnectionLifetime` property, so that DNS lookup is repeated when the connection is replaced. Consider the following example:

```csharp
var handler = new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
};
var sharedClient = new HttpClient(handler);
```

## Pooled Connections

The connection pool for an `HttpClient` is linked to the underlying `SocketsHttpHandler`. When the `HttpClient` instance is disposed, it disposes all existing connections inside the pool. If you later send a request to the same server, a new connection must be recreated. As a result, there's a performance penalty for unnecessary connection creation.

## Recommended Use

* Use a static or singleton `HttpClient` instance with `PooledConnectionLifetime` set to the desired interval, such as 2 minutes, depending on expected DNS changes. This solves both the port exhaustion and DNS changes problems without adding the overhead of `IHttpClientFactory`. If you need to be able to mock your handler, you can register it separately.

* Using `IHttpClientFactory`, you can have multiple, differently configured clients for different use cases. However, be aware that the factory-created clients are intended to be short-lived, and once the client is created, the factory no longer has control over it. The factory pools `HttpMessageHandler` (wrapper over `SocketsHttpHandler`) instances, and, if its lifetime hasn't expired, a handler can be reused from the pool when the factory creates a new `HttpClient` instance. This reuse avoids any socket exhaustion issues.

```csharp
class Service
{
    private readonly IHttpClientFactory _clientFactory;

    public Service(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task GetWebContent()
    {
        var client = _clientFactory.CreateClient();

        // Example: fetching content from a URL
        var response = await client.GetAsync("https://example.com");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
        else
        {
            Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
        }
    }
}
```

## Links

* https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0

#dotnet-http-client-connection-management
