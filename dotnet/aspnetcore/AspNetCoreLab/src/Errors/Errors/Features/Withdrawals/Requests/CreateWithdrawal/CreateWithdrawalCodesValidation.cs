using Errors.Errors.Exceptions;
using Errors.Storages;
using FluentValidation;
using MediatR;

using static Errors.Features.Withdrawals.Errors.Codes.WithdrawalValidationErrors;

namespace Errors.Features.Withdrawals.Requests.CreateWithdrawal;

public static class CreateWithdrawalCodesValidation
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
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!await UserStorage.Exists(request.UserName, cancellationToken))
            {
                throw new ValidationErrorsException($"{nameof(request.UserName)}", "User not found", UserNotFound);
            }

            if (!await AccountStorage.Exists(request.FromAccountNumber, cancellationToken))
            {
                throw new ValidationErrorsException($"{nameof(request.FromAccountNumber)}", "Account not found", AccountNotFound);
            }

            if (!await CurrencyStorage.Exists(request.Currency, cancellationToken))
            {
                throw new ValidationErrorsException($"{nameof(request.Currency)}", "Currency not found", CurrencyNotFound);
            }

            return Unit.Value;
        }
    }
}
