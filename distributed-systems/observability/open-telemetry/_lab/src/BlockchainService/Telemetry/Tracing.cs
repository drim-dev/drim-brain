using System.Diagnostics;

namespace BlockchainService.Telemetry;

public static class Tracing
{
    public static ActivitySource ActivitySource { get; private set; }

    public static void Init(string name)
    {
        ActivitySource = new ActivitySource(name);
    }
}
