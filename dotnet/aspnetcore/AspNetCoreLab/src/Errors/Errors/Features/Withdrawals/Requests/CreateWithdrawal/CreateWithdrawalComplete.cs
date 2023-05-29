using Errors.Errors.Exceptions;
using Errors.Validation;
using FluentValidation;
using MediatR;

using static Errors.Features.Withdrawals.Errors.Codes.WithdrawalValidationErrors;
using static Errors.Features.Withdrawals.Errors.Codes.WithdrawalLogicConfictErrors;

namespace Errors.Features.Withdrawals.Requests.CreateWithdrawal;

public static class CreateWithdrawalComplete
{
    public record Request(string FromAccountNumber, string ToAddress, string Currency, decimal Amount) : IRequest<Unit>
    {
        public string UserName { get; set; }
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserName).ValidAndExistingUserName();
            RuleFor(x => x.FromAccountNumber).ValidAndExistingAccountNumber();
            RuleFor(x => x.ToAddress).ValidCryptoAddress();
            RuleFor(x => x.Currency).ValidAndExistingCurrency();
            RuleFor(x => x.Amount).GreaterThan(0).WithErrorCode(AmountLessOrEqualZero);
        }
    }

    public class RequestHandler : IRequestHandler<Request, Unit>
    {
        public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.UserName == "seller")
            {
                throw new LogicConflictException("Withdrawals are disabled for this user", WithdrawalsDisabledForUser);
            }
            if (request.UserName == "owner")
            {
                throw new LogicConflictException("Withdrawals are globally disabled", WithdrawalsGloballyDisabled);
            }

            return Task.FromResult(Unit.Value);
        }
    }
}