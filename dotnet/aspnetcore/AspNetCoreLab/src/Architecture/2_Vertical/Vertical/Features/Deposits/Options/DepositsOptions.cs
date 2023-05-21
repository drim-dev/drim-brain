namespace Vertical.Features.Deposits.Options;

public class DepositsOptions
{
    public TimeSpan FindNewDepositsInterval { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan UpdateDepositConfirmationsInterval { get; set; } = TimeSpan.FromMinutes(1);
}
