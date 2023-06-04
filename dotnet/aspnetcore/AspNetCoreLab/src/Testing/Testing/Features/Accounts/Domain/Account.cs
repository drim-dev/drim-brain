using Testing.Features.Users.Domain;

namespace Testing.Features.Accounts.Domain;

public class Account
{
    public string Number { get; set; }

    public long UserId { get; set; }

    public User User { get; set; }

    public string Currency { get; set; }

    public decimal Amount { get; set; }
}
