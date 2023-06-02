namespace Errors.Features.Withdrawals.Errors.Codes;

public static class WithdrawalLogicConflictErrors
{
    private const string Prefix = "withdrawals_logic_confict_";

    public const string WithdrawalsGloballyDisabled = Prefix + "withdrawals_globally_disabled";
    public const string WithdrawalsDisabledForUser = Prefix + "withdrawals_disabled_for_user";
}
