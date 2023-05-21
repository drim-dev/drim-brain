using Clean.Options;
using Services.Configuration.Options;

namespace Clean.WebApplicationBuilderExtensions;

public static class OptionsBuilderExtensions
{
    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<DepositsOptions>(builder.Configuration.GetSection("Features:Deposits"));
        builder.Services.Configure<BitcoinNodeClientOptions>(builder.Configuration.GetSection("BitcoinNodeClient"));

        return builder;
    }
}
