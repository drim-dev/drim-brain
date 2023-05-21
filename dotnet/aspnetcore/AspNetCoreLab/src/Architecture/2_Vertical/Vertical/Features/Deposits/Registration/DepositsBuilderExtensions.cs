using Domain;
using Services.Configuration;
using Services.Configuration.Options;
using Vertical.Features.Deposits.Jobs.HostedServices;
using Vertical.Features.Deposits.Options;
using Vertical.Features.Deposits.Services;

namespace Vertical.Features.Deposits.Registration;

public static class DepositsBuilderExtensions
{
    public static WebApplicationBuilder AddDeposits(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<FindNewDepositsHostedService>();
        builder.Services.AddHostedService<UpdateDepositConfirmationsHostedService>();

        // Fake DbContext to satisfy service dependencies
        builder.Services.AddScoped<DbContext>();

        builder.Services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
        builder.Services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

        builder.Services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
        builder.Services.AddTransient<IDepositRepository, DepositRepository>();
        builder.Services.AddTransient<IAccountRepository, AccountRepository>();

        builder.Services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();

        builder.Services.AddTransient<CryptoAddressGenerator>();

        builder.Services.Configure<DepositsOptions>(builder.Configuration.GetSection("Features:Deposits"));

        // TODO: move to separate module
        builder.Services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();
        builder.Services.Configure<BitcoinNodeClientOptions>(builder.Configuration.GetSection("BitcoinNodeClient"));

        return builder;
    }
}
