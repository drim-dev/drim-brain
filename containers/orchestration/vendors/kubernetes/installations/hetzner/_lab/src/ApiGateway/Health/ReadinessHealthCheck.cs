using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateway.Health;

public class ReadinessHealthCheck : IHealthCheck
{
    public bool IsReady { get; set; } = false;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        var healthCheckResult = IsReady
            ? HealthCheckResult.Healthy("The API Gateway is ready to accept requests.")
            : HealthCheckResult.Unhealthy("The API Gateway is not ready to accept requests.");

        return Task.FromResult(healthCheckResult);
    }
}
