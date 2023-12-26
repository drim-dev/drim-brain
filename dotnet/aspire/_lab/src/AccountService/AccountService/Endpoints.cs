using AspireLab.ServiceDefaults;
using BlockchainService.Api;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WithdrawalDto = AccountService.Api.WithdrawalDto;

namespace AccountService;

public static class Endpoints
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapPost("/withdrawals", async Task<Ok> ([FromBody] WithdrawalDto withdrawal, BlockchainServiceClient client,
            ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
        {
            using (Tracing.ActivitySource.StartActivity("Sending withdrawal"))
            {
                await client.Withdraw(new(withdrawal.AccountId, withdrawal.Amount, withdrawal.CryptoAddress),
                    cancellationToken);
            }

            using (Tracing.ActivitySource.StartActivity("Deducting amount from account"))
            {
                await Task.Delay(Random.Shared.Next(10, 20), cancellationToken);

                var logger = loggerFactory.CreateLogger("AccountService");
                logger.LogInformation("Deducted amount from account");
            }

            return TypedResults.Ok();
        });

        return app;
    }
}
