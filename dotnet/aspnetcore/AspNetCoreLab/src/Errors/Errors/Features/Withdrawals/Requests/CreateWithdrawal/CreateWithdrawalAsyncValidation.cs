using Errors.Storages;
using FluentValidation;
using MediatR;

using static Errors.Features.Withdrawals.Errors.Codes.WithdrawalValidationErrors;

namespace Errors.Features.Withdrawals.Requests.CreateWithdrawal;

public static class CreateWithdrawalAsyncValidation
{
    public record Request(string FromAccountNumber, string ToAddress, string Currency, decimal Amount) : IRequest<Unit>
    {
        public string UserName { get; set; }
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithErrorCode(UserNameRequired)
                .MustAsync(async (name, ct) => await UserStorage.Exists(name, ct)).WithErrorCode(UserNotFound);
            RuleFor(x => x.FromAccountNumber)
                .NotEmpty().WithErrorCode(AccountNumberRequired)
                .MustAsync(async (number, ct) => await AccountStorage.Exists(number, ct)).WithErrorCode(AccountNotFound);
            RuleFor(x => x.ToAddress)
                .NotEmpty().WithErrorCode(AddressRequired);
            RuleFor(x => x.Currency)
                .NotEmpty().WithErrorCode(CurrencyRequired)
                .MustAsync(async (currency, ct) => await CurrencyStorage.Exists(currency, ct))
                .WithMessage("Currency not found")
                .WithErrorCode(CurrencyNotFound);
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithErrorCode(AmountLessOrEqualZero);
        }
    }

    public class RequestHandler : IRequestHandler<Request, Unit>
    {
        public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
