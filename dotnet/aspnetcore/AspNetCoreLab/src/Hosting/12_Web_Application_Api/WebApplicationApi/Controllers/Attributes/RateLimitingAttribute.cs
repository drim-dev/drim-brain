namespace WebApplicationApi.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RateLimitingAttribute : Attribute
{
    public RateLimitingAttribute(int intervalMs)
    {
        IntervalMs = intervalMs;
    }

    public int IntervalMs { get; }
}
