namespace Testing.Features.Accounts.Errors;

public static class AccountsValidationErrors
{
    private const string Prefix = "accounts_validation_";

    public const string UserIdRequired = Prefix + "user_id_required";
    public const string UserNotFound = Prefix + "user_not_found";
}
