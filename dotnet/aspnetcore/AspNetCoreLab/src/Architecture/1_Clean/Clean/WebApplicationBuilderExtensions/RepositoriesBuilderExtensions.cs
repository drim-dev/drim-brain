using Clean.Repositories;
using Clean.Repositories.Abstract;

namespace Clean.WebApplicationBuilderExtensions;

public static class RepositoriesBuilderExtensions
{
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<global::Services.Configuration.IDepositAddressRepository, global::Services.Configuration.DepositAddressRepository>();
        builder.Services.AddTransient<global::Services.Configuration.IDepositRepository, global::Services.Configuration.DepositRepository>();
        builder.Services.AddTransient<global::Services.Configuration.IAccountRepository, global::Services.Configuration.AccountRepository>();

        builder.Services.AddTransient<IAccountRepository, AccountRepository>();
        builder.Services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
        builder.Services.AddTransient<IDepositRepository, DepositRepository>();
        builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IWithdrawalRepository, WithdrawalRepository>();

        return builder;
    }
}
