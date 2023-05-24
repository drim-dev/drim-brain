using FluentValidation;
using MediatR;

namespace Auth.Features.Payments.Requests;

public static class GenerateQrCode
{
    public record Request(decimal Amount) : IRequest<Response>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }

    public record Response(string QrCode);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Response($"QrCodeFor{request.Amount}Btc"));
        }
    }
}
