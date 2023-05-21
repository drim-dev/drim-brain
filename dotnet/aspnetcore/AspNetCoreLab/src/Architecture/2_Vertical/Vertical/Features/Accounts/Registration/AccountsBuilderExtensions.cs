using Vertical.Features.Accounts.Options;

namespace Vertical.Features.Accounts.Registration;

public static class AccountsBuilderExtensions
{
    public static WebApplicationBuilder AddAccounts(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AccountsOptions>(builder.Configuration.GetSection("Features:Accounts"));

        return builder;
    }
}
