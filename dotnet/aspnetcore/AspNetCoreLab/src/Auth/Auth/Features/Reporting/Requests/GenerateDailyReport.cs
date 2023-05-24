using MediatR;

namespace Auth.Features.Reporting.Requests;

public static class GenerateDailyReport
{
    public record Request : IRequest<Response>;

    public record Response(string Report);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Response("Daily report content"));
        }
    }
}
