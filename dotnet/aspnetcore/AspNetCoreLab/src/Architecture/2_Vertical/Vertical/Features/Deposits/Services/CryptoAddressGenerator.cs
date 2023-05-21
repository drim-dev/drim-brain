namespace Vertical.Features.Deposits.Services;

public class CryptoAddressGenerator
{
    public string GenerateAddress()
    {
        return Guid.NewGuid().ToString();
    }
}
