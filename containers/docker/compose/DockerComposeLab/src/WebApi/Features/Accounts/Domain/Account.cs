namespace WebApi.Features.Accounts.Domain;

public class Account
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CurrencyCode { get; set; }

    public decimal Amount { get; set; }

    public DateTime OpenedAt { get; set; }
}
