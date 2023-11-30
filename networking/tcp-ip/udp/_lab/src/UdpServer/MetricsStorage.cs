using Metrics;

namespace UdpServer;

public class MetricsStorage
{
    private readonly List<EngineMetrics> _engineMetrics = new();

    public void Save(EngineMetrics metrics)
    {
        _engineMetrics.Add(metrics);
    }
}
