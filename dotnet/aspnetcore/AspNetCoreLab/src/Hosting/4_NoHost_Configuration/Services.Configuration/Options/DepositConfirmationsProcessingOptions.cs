namespace Services.Configuration.Options;

public class DepositConfirmationsProcessingOptions
{
    public TimeSpan Interval { get; set; }
    public int MinConfirmations { get; set; }
}