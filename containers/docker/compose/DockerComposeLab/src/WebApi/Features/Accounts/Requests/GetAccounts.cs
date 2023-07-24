using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Features.Accounts.Models;

namespace WebApi.Features.Accounts.Requests;

public static class GetAccounts
{
    [AllowAnonymous]
    [HttpGet("/accounts")]
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator) => _mediator = mediator;

        public override Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken) =>
            _mediator.Send(request, cancellationToken);
    }

    public record Request(int UserId) : IRequest<Response>;

    public record Response(ICollection<AccountModel> Accounts);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken) =>
            new(await _db.Accounts
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.OpenedAt)
                .Select(x => new AccountModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CurrencyCode = x.CurrencyCode,
                    Amount = x.Amount,
                    OpenedAt = x.OpenedAt,
                })
                .ToListAsync(cancellationToken));
    }
}
