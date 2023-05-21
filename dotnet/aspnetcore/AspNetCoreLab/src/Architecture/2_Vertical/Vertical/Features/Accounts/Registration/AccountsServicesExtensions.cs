using Vertical.Features.Accounts.Options;

namespace Vertical.Features.Deposits.Registration;

public static class AccountsServicesExtensions
{
    public static WebApplicationBuilder AddAccounts(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AccountsOptions>(builder.Configuration.GetSection("Features:Accounts"));

        return builder;
    }
}
