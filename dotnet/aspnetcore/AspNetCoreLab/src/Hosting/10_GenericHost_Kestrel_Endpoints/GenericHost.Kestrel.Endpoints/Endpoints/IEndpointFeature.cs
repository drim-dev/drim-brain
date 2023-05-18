namespace GenericHost.Kestrel.Endpoints.Endpoints;

public class EndpointFeature
{
    public EndpointFeature(Endpoint? endpoint)
    {
        Endpoint = endpoint;
    }

    public Endpoint? Endpoint { get; }
}