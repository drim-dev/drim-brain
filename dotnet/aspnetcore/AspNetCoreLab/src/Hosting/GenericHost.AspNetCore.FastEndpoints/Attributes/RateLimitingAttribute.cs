namespace GenericHost.AspNetCore.FastEndpoints.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitingAttribute : Attribute
{
    public RateLimitingAttribute(int intervalMs)
    {
        IntervalMs = intervalMs;
    }

    public int IntervalMs { get; }
}
