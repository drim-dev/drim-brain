using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateway.Health;

public class LivenessHealthCheck : IHealthCheck
{
    public bool IsAlive { get; set; } = true;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        var healthCheckResult = IsAlive
            ? HealthCheckResult.Healthy("The API Gateway is alive and well.")
            : HealthCheckResult.Unhealthy("The API Gateway is not alive.");

        return Task.FromResult(healthCheckResult);
    }
}
