using FluentValidation;
using MediatR;

namespace Errors.Features.Withdrawals.Requests.CreateWithdrawal;

public static class CreateWithdrawalBasicValidation
{
    public record Request(string AccountNumber, string ToAddress, string Currency, decimal Amount) : IRequest<Unit>
    {
        public string UserName { get; set; }
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.AccountNumber).NotEmpty();
            RuleFor(x => x.ToAddress).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
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
