namespace ApiGateway.Features.Withdrawals.Models;

// TODO: use decimal for Amount
public record WithdrawalModel(long Id, string AccountNumber, string Currency, double Amount, string CryptoAddress,
    string TxId, DateTime CreatedAt);
