using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Endpoints;

public record HttpApplicationContext(IFeatureCollection Features);
