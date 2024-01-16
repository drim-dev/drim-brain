namespace Concurrency.Features.Withdrawals.Domain;

public class Withdrawal
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int AccountId { get; set; }

    public string CurrencyCode { get; set; }

    public decimal Amount { get; set; }

    public string ToAddress { get; set; }

    public DateTime CreatedAt { get; set; }
}
