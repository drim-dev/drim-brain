using MediatR;

namespace Auth.Features.Shifts.Requests;

public static class CloseShift
{
    public record Request : IRequest<Response>;

    public record Response(string Message);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Response("Shift closed"));
        }
    }
}
