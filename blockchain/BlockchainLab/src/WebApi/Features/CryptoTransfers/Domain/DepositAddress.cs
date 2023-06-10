using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Features.Currencies.Domain;
using WebApi.Features.Users.Domain;

namespace WebApi.Features.CryptoTransfers.Domain;

public class DepositAddress
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Guid { get; private set; }
    public Guid CurrencyId { get; private set; }
    public Currency? Currency { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public Guid XpubId { get; private set; }
    public Xpub? Xpub { get; private set; }
    public int DerivationIndex { get; private set; }
    public string CryptoAddress { get; private set; }
    public DateTime DateOfCreation { get; set; }

    public DepositAddress(Guid currencyId, Guid userId, Guid xpubId, int derivationIndex, string cryptoAddress)
    {
        CurrencyId = currencyId;
        UserId = userId;
        XpubId = xpubId;
        DerivationIndex = derivationIndex;
        CryptoAddress = cryptoAddress;
        DateOfCreation = DateTime.Now;
    }
}
