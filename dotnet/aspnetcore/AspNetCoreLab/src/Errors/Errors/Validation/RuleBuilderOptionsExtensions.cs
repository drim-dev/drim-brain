using Errors.Storages;
using FluentValidation;

using static Errors.Features.Withdrawals.Errors.Codes.WithdrawalValidationErrors;

namespace Errors.Validation;

public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<T, string> ValidAndExistingUserName<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithErrorCode(UserNameRequired)
            .MustAsync(async (name, ct) => await UserStorage.Exists(name, ct)).WithErrorCode(UserNotFound);
    }

    public static IRuleBuilderOptions<T, string> ValidAndExistingAccountNumber<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithErrorCode(AccountNumberRequired)
            .MustAsync(async (number, ct) => await AccountStorage.Exists(number, ct)).WithErrorCode(AccountNotFound);
    }

    public static IRuleBuilderOptions<T, string> ValidAndExistingCurrency<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithErrorCode(CurrencyRequired)
            .MustAsync(async (currency, ct) => await CurrencyStorage.Exists(currency, ct)).WithErrorCode(CurrencyNotFound);
    }

    public static IRuleBuilderOptions<T, string> ValidCryptoAddress<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithErrorCode(AddressRequired)
            .Must(x => x.StartsWith("0x")).WithErrorCode(AddressInvalid);
    }
}
