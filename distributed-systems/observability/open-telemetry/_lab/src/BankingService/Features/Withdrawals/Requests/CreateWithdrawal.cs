using BlockchainService.Client;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace BankingService.Features.Withdrawals.Requests;

internal static class CreateWithdrawal
{
    internal class RequestValidator : AbstractValidator<CreateWithdrawalRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountNumber).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.CryptoAddress).NotEmpty();
        }
    }

    internal class RequestHandler(BlockchainService.Client.Withdrawals.WithdrawalsClient _withdrawalsClient)
        : IRequestHandler<CreateWithdrawalRequest, CreateWithdrawalReply>
    {
        public async Task<CreateWithdrawalReply> Handle(CreateWithdrawalRequest request, CancellationToken cancellationToken)
        {
            var withdrawRequest = new WithdrawRequest
            {
                Currency = request.Currency,
                Amount = request.Amount,
                CryptoAddress = request.CryptoAddress,
            };

            var withdrawResponse = await _withdrawalsClient.WithdrawAsync(withdrawRequest,
                new CallOptions(cancellationToken: cancellationToken));

            return new()
            {
                Withdrawal = new()
                {
                    Id = 1,
                    UserId = request.UserId,
                    AccountNumber = request.AccountNumber,
                    Currency = request.Currency,
                    Amount = request.Amount,
                    CryptoAddress = request.CryptoAddress,
                    TxId = withdrawResponse.TxId,
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                }
            };
        }
    }
}
