namespace Testing.Features.Users.Errors;

public static class UsersValidationErrors
{
    private const string Prefix = "users_validation_";

    public const string EmailRequired = Prefix + "email_required";
    public const string PasswordRequired = Prefix + "password_required";
    public const string InvalidEmailFormat = Prefix + "invalid_email_format";
    public const string InvalidPasswordLength = Prefix + "invalid_password_length";
    public const string EmailAlreadyTaken = Prefix + "email_already_taken";
    public const string TooOld = Prefix + "too_old";
    public const string TooYoung = Prefix + "too_young";
}
