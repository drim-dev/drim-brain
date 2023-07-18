using Transactions.Features.Withdrawals.Jobs;
using Transactions.Features.Withdrawals.Options;
using Transactions.Features.Withdrawals.Services;

namespace Transactions.Features.Withdrawals.Extensions;

public static class WithdrawalsWebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddWithdrawals(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<WithdrawalsOptions>(builder.Configuration.GetSection("Features:Withdrawals"));

        builder.Services.AddScoped<CryptoSender>();

        builder.Services.AddHostedService<WithdrawalOutboxHostedService>();

        return builder;
    }
}
