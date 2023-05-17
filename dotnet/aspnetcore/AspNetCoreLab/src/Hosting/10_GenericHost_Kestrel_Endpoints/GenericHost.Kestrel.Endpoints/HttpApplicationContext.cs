using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.Endpoints;

internal record HttpApplicationContext(IFeatureCollection Features);
