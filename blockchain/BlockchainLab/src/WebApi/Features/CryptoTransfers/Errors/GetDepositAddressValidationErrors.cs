namespace WebApi.Features.CryptoTransfers.Errors;

public static class GetDepositAddressValidationErrors
{
    private const string Prefix = "deposit_address_validation_";

    public const string UserIdRequired = Prefix + "user_id_required";
    public const string UserNotFound = Prefix + "user_not_found";
    public const string CurrencyCodeRequired = Prefix + "currency_code_required";
}
