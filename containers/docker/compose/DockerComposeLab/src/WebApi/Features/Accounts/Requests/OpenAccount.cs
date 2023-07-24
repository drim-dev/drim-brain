using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using WebApi.Database;
using WebApi.Features.Accounts.Domain;

namespace WebApi.Features.Accounts.Requests;

public static class OpenAccount
{
    [AllowAnonymous]
    [HttpPost("/accounts")]
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator) => _mediator = mediator;

        public override Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken) =>
            _mediator.Send(request, cancellationToken);
    }

    public record Request(int UserId, string CurrencyCode) : IRequest<Response>;

    public record Response(int AccountId);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                UserId = request.UserId,
                CurrencyCode = request.CurrencyCode,
                Amount = 0,
                OpenedAt = DateTime.UtcNow,
            };

            _db.Accounts.Add(account);

            await _db.SaveChangesAsync(cancellationToken);

            return new(account.Id);
        }
    }
}
