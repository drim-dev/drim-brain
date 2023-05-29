namespace Errors.Features.Withdrawals.Errors.Codes;

public static class WithdrawalValidationErrors
{
    private const string Prefix = "withdrawals_validation_";

    public const string UserNameRequired = Prefix + "user_name_required";
    public const string UserNotFound = Prefix + "user_not_found";
    public const string AccountNumberRequired = Prefix + "account_number_required";
    public const string AccountNotFound = Prefix + "account_not_found";
    public const string AddressRequired = Prefix + "address_number_required";
    public const string AddressInvalid = Prefix + "address_invalid";
    public const string CurrencyRequired = Prefix + "currency_required";
    public const string CurrencyNotFound = Prefix + "currency_not_found";
    public const string AmountLessOrEqualZero = Prefix + "amount_less_or_equal_zero";
}
