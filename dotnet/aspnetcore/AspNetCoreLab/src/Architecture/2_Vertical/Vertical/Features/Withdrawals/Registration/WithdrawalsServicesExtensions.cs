using Vertical.Features.Withdrawals.Options;

namespace Vertical.Features.Deposits.Registration;

public static class WithdrawalsServicesExtensions
{
    public static WebApplicationBuilder AddWithdrawals(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<WithdrawalsOptions>(builder.Configuration.GetSection("Features:Withdrawals"));

        return builder;
    }
}
