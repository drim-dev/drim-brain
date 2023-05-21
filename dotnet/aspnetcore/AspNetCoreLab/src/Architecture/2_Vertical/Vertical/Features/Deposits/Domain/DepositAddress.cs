namespace Vertical.Features.Deposits.Domain;

public class DepositAddress
{
    public int Id { get; set; }
    public string CurrencyCode { get; set; }
    public int UserId { get; set; }
    public int XpubId { get; set; }
    public int DerivationIndex { get; set; }
    public string CryptoAddress { get; set; }
}
