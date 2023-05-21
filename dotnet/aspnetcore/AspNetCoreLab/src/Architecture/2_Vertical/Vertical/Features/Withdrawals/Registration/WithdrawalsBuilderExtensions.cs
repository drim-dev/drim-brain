using Vertical.Features.Withdrawals.Options;

namespace Vertical.Features.Withdrawals.Registration;

public static class WithdrawalsBuilderExtensions
{
    public static WebApplicationBuilder AddWithdrawals(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<WithdrawalsOptions>(builder.Configuration.GetSection("Features:Withdrawals"));

        return builder;
    }
}
