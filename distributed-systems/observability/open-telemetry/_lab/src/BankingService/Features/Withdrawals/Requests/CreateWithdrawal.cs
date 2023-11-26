using FluentValidation;
using Google.Protobuf.WellKnownTypes;
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

    internal class RequestHandler : IRequestHandler<CreateWithdrawalRequest, CreateWithdrawalReply>
    {
        public Task<CreateWithdrawalReply> Handle(CreateWithdrawalRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CreateWithdrawalReply
            {
                Withdrawal = new()
                {
                    Id = 1,
                    UserId = request.UserId,
                    AccountNumber = request.AccountNumber,
                    Currency = request.Currency,
                    Amount = request.Amount,
                    CryptoAddress = request.CryptoAddress,
                    TxId = "txid",
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                }
            });
        }
    }
}
