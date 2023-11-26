using ApiGateway.Features.Withdrawals.Models;
using Clients.BankingService;
using Common.Web.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Features.Withdrawals.Requests;

public static class CreateWithdrawal
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/withdrawals";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, async Task<Created<WithdrawalModel>>
                    (RequestBody body, [FromServices]Clients.BankingService.Withdrawals.WithdrawalsClient withdrawalsClient, CancellationToken cancellationToken) =>
                {
                    var reply = await withdrawalsClient.CreateWithdrawalAsync(new CreateWithdrawalRequest
                    {
                        UserId = body.UserId,
                        AccountNumber = body.AccountNumber,
                        Currency = body.Currency,
                        Amount = body.Amount,
                        CryptoAddress = body.CryptoAddress,
                    });

                    var withdrawal = reply.Withdrawal;
                    var withdrawalModel = new WithdrawalModel(withdrawal.Id, withdrawal.AccountNumber,
                        withdrawal.Currency, withdrawal.Amount, withdrawal.CryptoAddress, withdrawal.TxId,
                        withdrawal.CreatedAt.ToDateTime());

                    return TypedResults.Created($"{Path}/{withdrawalModel.Id}", withdrawalModel);
                })
                .AllowAnonymous();
        }

        // TODO: get UserId from JWT
        // TODO: use decimal for Amount
        private record RequestBody(int UserId, string AccountNumber, string Currency, double Amount, string CryptoAddress);
    }
}
