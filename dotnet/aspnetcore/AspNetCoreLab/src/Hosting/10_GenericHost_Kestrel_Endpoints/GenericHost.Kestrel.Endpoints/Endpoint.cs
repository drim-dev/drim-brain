namespace GenericHost.Kestrel.Endpoints;

public record Endpoint(string PathPattern, Func<HttpApplicationContext, IServiceScope, Task> EndpointDelegate, Dictionary<string, object>? Metadata = null);

public class EndpointCollection : List<Endpoint>
{
    public EndpointCollection()
    {
    }
}

public static class EndpointMetadataKeys
{
    public const string RateLimitingInverval = nameof(RateLimitingInverval);
}