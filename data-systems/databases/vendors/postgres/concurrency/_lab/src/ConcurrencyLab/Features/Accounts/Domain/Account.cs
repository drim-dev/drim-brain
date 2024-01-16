namespace Concurrency.Features.Accounts.Domain;

public class Account
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CurrencyCode { get; set; }

    public decimal Amount { get; set; }

    public DateTime OpenedAt { get; set; }
}

public class VersionedAccount
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CurrencyCode { get; set; }

    public decimal Amount { get; set; }

    public DateTime OpenedAt { get; set; }

    public uint Version { get; set; }
}