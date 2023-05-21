using Clean.Services;
using Clean.Services.Abstract;
using Services.Configuration;

namespace Clean.WebApplicationBuilderExtensions;

public static class ServicesBuilderExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        // Fake DbContext to satisfy service dependencies
        builder.Services.AddScoped<global::Domain.DbContext>();

        builder.Services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
        builder.Services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

        builder.Services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();
        builder.Services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();

        builder.Services.AddTransient<IAccountService, AccountService>();
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<IDepositService, DepositService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IWithdrawalService, WithdrawalService>();

        return builder;
    }
}
