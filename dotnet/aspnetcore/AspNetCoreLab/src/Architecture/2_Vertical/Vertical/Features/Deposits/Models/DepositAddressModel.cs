namespace Vertical.Features.Deposits.Models;

public class DepositAddressModel
{
    public DepositAddressModel(string cryptoAddress)
    {
        CryptoAddress = cryptoAddress;
    }

    public string CryptoAddress { get; }
}
