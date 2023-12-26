using AccountService.Api;
using LoanService.Api;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiGateway;

public static class Endpoints
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/loans/offerings",
            async Task<Ok<LoanOfferingDto[]>> (LoanServiceClient client, CancellationToken cancellationToken) =>
            {
                var offerings = await client.GetOfferings(cancellationToken);
                return TypedResults.Ok(offerings);
            })
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)));

        app.MapPost("/accounts/withdrawals",
            async Task<Ok> (WithdrawalDto withdrawal, AccountServiceClient client, CancellationToken cancellationToken) =>
            {
                await client.Withdraw(withdrawal, cancellationToken);
                return TypedResults.Ok();
            });

        return app;
    }
}
