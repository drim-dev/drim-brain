namespace BankingService.Domain;

public class Withdrawal(long id, int userId, string accountNumber, string currency, double amount, string cryptoAddress,
    string txId, DateTime createdAt)
{
    public long Id { get; } = id;

    public int UserId { get; } = userId;

    public string AccountNumber { get; } = accountNumber;

    public string Currency { get; } = currency;

    public double Amount { get; } = amount;

    public string CryptoAddress { get; } = cryptoAddress;

    public string TxId { get; } = txId;

    public DateTime CreatedAt { get; } = createdAt;
}
