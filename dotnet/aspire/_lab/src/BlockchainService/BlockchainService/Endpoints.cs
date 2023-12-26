using BlockchainService.Api;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainService;

public static class Endpoints
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapPost("/withdrawals", async Task<Ok> ([FromBody] WithdrawalDto withdrawal, ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            await Task.Delay(Random.Shared.Next(30, 100), cancellationToken);

            var logger = loggerFactory.CreateLogger("BlockchainService");
            logger.LogInformation("Sent amount {Amount} to crypto address {CryptoAddress}", withdrawal.Amount,
                withdrawal.CryptoAddress);

            return TypedResults.Ok();
        });

        return app;
    }
}
