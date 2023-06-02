using Errors.Errors.Exceptions;
using FluentValidation;
using MediatR;

using static Errors.Features.Withdrawals.Errors.Codes.WithdrawalValidationErrors;
using static Errors.Features.Withdrawals.Errors.Codes.WithdrawalLogicConflictErrors;

namespace Errors.Features.Withdrawals.Requests.CreateWithdrawal;

public static class CreateWithdrawalLogicConflict
{
    public record Request(string FromAccountNumber, string ToAddress, string Currency, decimal Amount) : IRequest<Unit>
    {
        public string UserName { get; set; }
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithErrorCode(UserNameRequired);
            RuleFor(x => x.FromAccountNumber).NotEmpty().WithErrorCode(AccountNumberRequired);
            RuleFor(x => x.ToAddress).NotEmpty().WithErrorCode(AddressRequired);
            RuleFor(x => x.Currency).NotEmpty().WithErrorCode(CurrencyRequired);
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
