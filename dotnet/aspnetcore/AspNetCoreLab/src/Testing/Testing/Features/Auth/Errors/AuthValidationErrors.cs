namespace Testing.Features.Auth.Errors;

public static class AuthValidationErrors
{
    private const string Prefix = "auth_validation_";

    public const string EmailRequired = Prefix + "email_required";
    public const string PasswordRequired = Prefix + "password_required";
    public const string InvalidEmailFormat = Prefix + "invalid_email_format";
    public const string WrongCredentials = Prefix + "wrong_credentials";
}
