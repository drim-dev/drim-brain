using FluentValidation;
using MediatR;

namespace Testing.Features.Deposits.Requests;

public static class GetDepositAddress
{
    public record Request(long UserId, string Currency) : IRequest<Response>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
        }
    }

    public record Response(string Address);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        public RequestHandler()
        {
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
