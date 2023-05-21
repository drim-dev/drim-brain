using Clean.HostedServices;

namespace Clean.WebApplicationBuilderExtensions;

public static class HostedServicesBuilderExtensions
{
    public static WebApplicationBuilder AddHostedServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<NewDepositHostedService>();
        builder.Services.AddHostedService<DepositConfirmationsHostedService>();

        return builder;
    }
}
