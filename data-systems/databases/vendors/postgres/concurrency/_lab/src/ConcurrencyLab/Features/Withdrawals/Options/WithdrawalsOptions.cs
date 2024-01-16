namespace Concurrency.Features.Withdrawals.Options;

public class WithdrawalsOptions
{
    public decimal DailyLimit { get; set; } = 100;

    public TimeSpan OutboxProcessingInterval { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan OutboxRecordProcessingTimeout { get; set; } = TimeSpan.FromSeconds(10);
}
