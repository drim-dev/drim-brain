namespace Transactions.Features.Withdrawals.Domain;

public class WithdrawalOutboxRecord
{
    public int Id { get; set; }

    public int WithdrawalId { get; set; }

    public Withdrawal Withdrawal { get; set; }

    public DateTime CreatedAt { get; set; }

    public WithdrawalOutboxRecordStatus Status { get; set; }
}

public enum WithdrawalOutboxRecordStatus
{
    NotSent,
    Sent,
    ManualReview,
}
