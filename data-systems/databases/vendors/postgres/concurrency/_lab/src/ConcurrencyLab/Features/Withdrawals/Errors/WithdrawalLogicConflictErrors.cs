namespace Concurrency.Features.Withdrawals.Errors;

public static class WithdrawalLogicConflictErrors
{
    private const string Prefix = "accounts_logic_conflict_";

    public const string AccountDailyWithdrawalLimitExceeded = Prefix + "daily_withdrawal_limit_exceeded";
}
