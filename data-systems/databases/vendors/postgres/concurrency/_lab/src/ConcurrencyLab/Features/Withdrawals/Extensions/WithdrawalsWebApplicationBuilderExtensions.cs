using Concurrency.Features.Withdrawals.Jobs;
using Concurrency.Features.Withdrawals.Options;
using Concurrency.Features.Withdrawals.Services;

namespace Concurrency.Features.Withdrawals.Extensions;

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
