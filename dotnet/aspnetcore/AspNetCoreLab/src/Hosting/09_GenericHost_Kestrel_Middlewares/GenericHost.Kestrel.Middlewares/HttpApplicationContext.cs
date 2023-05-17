using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Middlewares;

public record HttpApplicationContext(IFeatureCollection Features);
