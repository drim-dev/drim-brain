namespace BlockchainService.Features.Withdrawals.Registration;

public static class WithdrawalsRegistrationExtensions
{
    public static WebApplicationBuilder AddWithdrawals(this WebApplicationBuilder builder)
    {
        return builder;
    }

    public static WebApplication MapWithdrawals(this WebApplication app)
    {
        app.MapGrpcService<WithdrawalsApi>();

        return app;
    }
}
