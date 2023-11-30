using MessagePack;

namespace Metrics;

[MessagePackObject]
public record EngineMetrics(
    [property: Key(0)] int EngineId,
    [property: Key(1)] decimal CombustionTemperature,
    [property: Key(2)] decimal CombustionPressure,
    [property: Key(3)] decimal OxygenConsumption,
    [property: Key(4)] decimal MethaneConsumption);
