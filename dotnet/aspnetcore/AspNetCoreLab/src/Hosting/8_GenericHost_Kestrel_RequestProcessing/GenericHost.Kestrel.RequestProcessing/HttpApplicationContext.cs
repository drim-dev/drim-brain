using Microsoft.AspNetCore.Http.Features;

namespace GenericHost.Kestrel.RequestProcessing;

internal record HttpApplicationContext(IFeatureCollection Features);
