namespace WebApi.Features.Accounts.Models;

public class AccountModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CurrencyCode { get; set; }

    public decimal Amount { get; set; }

    public DateTime OpenedAt { get; set; }
}
