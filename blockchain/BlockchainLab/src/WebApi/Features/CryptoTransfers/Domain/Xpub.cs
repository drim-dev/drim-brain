using WebApi.Features.Currencies.Domain;

namespace WebApi.Features.CryptoTransfers.Domain;

public class Xpub
{
    public Guid Id { get; private set; }
    public Guid CurrencyId { get; private set; }
    public Currency? Currency { get; private set; }
    public string Value { get; private set; }
}
