using GenericHost.Kestrel.Endpoints.HostedServices;

namespace GenericHost.Kestrel.Endpoints.Endpoints;

public record Endpoint(string PathPattern, Func<HttpApplicationContext, IServiceScope, Task> EndpointDelegate, Dictionary<string, object>? Metadata = null);

public class EndpointCollection : List<Endpoint>
{
}

public static class EndpointMetadataKeys
{
    public const string RateLimitingInverval = nameof(RateLimitingInverval);
}