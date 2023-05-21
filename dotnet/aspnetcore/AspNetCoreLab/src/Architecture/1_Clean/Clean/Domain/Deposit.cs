namespace Clean.Domain;

public class Deposit
{
    public ulong Id { get; set; }
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public DepositAddress Address { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; }
    public string TxId { get; set; }
    public uint Confirmations { get; set; }
    public DepositStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum DepositStatus
{
    Created,
    Confirmed,
}
