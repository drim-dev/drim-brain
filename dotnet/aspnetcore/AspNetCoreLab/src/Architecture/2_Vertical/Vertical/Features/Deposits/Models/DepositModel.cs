using Vertical.Features.Deposits.Domain;

namespace Vertical.Features.Deposits.Models;

public class DepositModel
{
    public ulong Id { get; set; }
    public string CryptoAddress { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; }
    public string TxId { get; set; }
    public uint Confirmations { get; set; }
    public DepositStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
