using BankingService.Database;
using BankingService.Domain;
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

    internal class RequestHandler(
        BlockchainService.Client.Withdrawals.WithdrawalsClient _withdrawalsClient,
        BankingDbContext _db,
        ILogger<RequestHandler> _logger)
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

            var withdrawal = new Withdrawal(0, request.UserId, request.AccountNumber, request.Currency, request.Amount,
                request.CryptoAddress, withdrawResponse.TxId, DateTime.UtcNow);

            await _db.Withdrawals.AddAsync(withdrawal, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Withdrawal created");

            return new() { Withdrawal = MapFrom(withdrawal) };

            static WithdrawalDto MapFrom(Withdrawal withdrawal) =>
                new()
                {
                    Id = withdrawal.Id,
                    UserId = withdrawal.UserId,
                    AccountNumber = withdrawal.AccountNumber,
                    Currency = withdrawal.Currency,
                    Amount = withdrawal.Amount,
                    CryptoAddress = withdrawal.CryptoAddress,
                    TxId = withdrawal.TxId,
                    CreatedAt = Timestamp.FromDateTime(withdrawal.CreatedAt),
                };
        }
    }
}
