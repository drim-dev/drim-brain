using System.Diagnostics.Metrics;

namespace ApiGateway.Metrics;

public class ApiGatewayMetrics
{
    public const string MeterName = "ApiGateway";

    private readonly Counter<int> _withdrawalsCreated;

    public ApiGatewayMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _withdrawalsCreated = meter.CreateCounter<int>("api_gateway.withdrawals.created");
    }

    public void WithdrawalsCreated(int count)
    {
        _withdrawalsCreated.Add(count);
    }
}
