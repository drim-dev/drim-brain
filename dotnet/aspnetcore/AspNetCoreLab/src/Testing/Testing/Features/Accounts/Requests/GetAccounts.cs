using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Testing.Database;
using Testing.Features.Accounts.Models;

namespace Testing.Features.Accounts.Requests;

public static class GetAccounts
{
    public record Request(long UserId) : IRequest<Response>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(TestingDbContext db)
        {
            RuleFor(x => x.UserId).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (userId, ct) => await db.Users.AnyAsync(x => x.Id == userId, ct));
        }
    }

    public record Response(AccountModel[] Accounts);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly TestingDbContext _db;

        public RequestHandler(TestingDbContext db) => _db = db;

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var accounts = await _db.Accounts
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.Number)
                .Select(x => new AccountModel
                {
                    Number = x.Number,
                    Currency = x.Currency,
                    Amount = x.Amount
                })
                .ToArrayAsync(cancellationToken);

            return new Response(accounts);
        }
    }
}
