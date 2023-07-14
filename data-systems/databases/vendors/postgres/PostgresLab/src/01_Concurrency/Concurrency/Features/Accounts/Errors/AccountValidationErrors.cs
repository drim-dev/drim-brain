namespace Transactions.Features.Accounts.Errors;

public static class AccountValidationErrors
{
    private const string Prefix = "accounts_validation_";

    public const string AccountNotExists = Prefix + "account_not_exists";
    public const string AccountAmountNotEnough = Prefix + "account_amount_not_enough";
}
