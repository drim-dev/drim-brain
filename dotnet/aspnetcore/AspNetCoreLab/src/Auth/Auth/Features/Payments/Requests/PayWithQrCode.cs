using FluentValidation;
using MediatR;

namespace Auth.Features.Payments.Requests;

public static class PayWithQrCode
{
    public record Request(string QrCode) : IRequest<Response>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.QrCode).NotEmpty();
        }
    }

    public record Response(string Message);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Response("Payment done"));
        }
    }
}
