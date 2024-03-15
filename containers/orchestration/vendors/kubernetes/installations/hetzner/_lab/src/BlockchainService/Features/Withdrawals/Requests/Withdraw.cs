using FluentValidation;
using MediatR;

namespace BlockchainService.Features.Withdrawals.Requests;

internal static class Withdraw
{
    internal class RequestValidator : AbstractValidator<WithdrawRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Currency).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.CryptoAddress).NotEmpty();
        }
    }

    internal class RequestHandler(ILogger<RequestHandler> _logger) : IRequestHandler<WithdrawRequest, WithdrawReply>
    {
        public async Task<WithdrawReply> Handle(WithdrawRequest request, CancellationToken cancellationToken)
        {
            await EstimateFee(cancellationToken);

            var txId = await SendTransaction(cancellationToken);

            return new() { TxId = txId };
        }

        private async Task EstimateFee(CancellationToken cancellationToken)
        {
            await Task.Delay(Random.Shared.Next(5, 10), cancellationToken);

            _logger.LogInformation("Fee estimated");
        }

        private async Task<string> SendTransaction(CancellationToken cancellationToken)
        {
            await Task.Delay(Random.Shared.Next(50, 100), cancellationToken);

            _logger.LogInformation("Transaction sent");

            return Guid.NewGuid().ToString();
        }
    }
}
