using Prometheus;

namespace Clean.Observability;

public static class HandlerMetrics
{
    private const string MessageTypeLabel = "messageType";

    private static readonly Counter TotalCounter = Metrics.CreateCounter(
        "handler_execution_total", "The count of total handler executions.",
        MessageTypeLabel);

    private static readonly Counter SuccessCounter = Metrics.CreateCounter(
        "handler_execution_success_total", "The count of successful handler executions.",
        MessageTypeLabel);

    private static readonly Counter FailureCounter = Metrics.CreateCounter(
        "handler_execution_failure_total", "The count of failed handler executions.",
        MessageTypeLabel);

    private static readonly Histogram ExecutionDuration = Metrics.CreateHistogram(
        "handler_execution_duration_seconds", "The duration of handler execution.",
        new HistogramConfiguration
        {
            Buckets = new[] {0.000001, 0.000002, 0.000004, 0.000008, 0.00001, 0.00002, 0.00004, 0.00008, 0.0001, 0.0002, 0.0004, 0.0008,
                0.001, 0.002, 0.004, 0.008, 0.015, 0.03, 0.05, 0.08, 0.1, 0.15, 0.2, 0.4, 0.8, 1.0, 1.5, 2.0, 5.0, 10.0, 30.0, 60.0},
            LabelNames = new[] {MessageTypeLabel}
        });

    public static async Task<TResponse> Meter<TRequest, TResponse>(Func<Task<TResponse>> wrapped)
    {
        var requestType = typeof(TRequest).FullName!;

        using var _ = ExecutionDuration.WithLabels(requestType).NewTimer();

        try
        {
            TotalCounter.WithLabels(requestType).Inc();

            var response = await wrapped();

            SuccessCounter.WithLabels(requestType).Inc();

            return response;
        }
        catch
        {
            FailureCounter.WithLabels(requestType).Inc();

            throw;
        }
    }
}
