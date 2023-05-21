using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vertical.Database;
using Vertical.Features.Deposits.Models;

namespace Vertical.Features.Deposits.Requests;

public static class GetDeposits
{
    // TODO: add filtering, sorting and pagination
    public record Request(int UserId) : IRequest<Response>;

    public record Response(DepositModel[] Deposits);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(0);
        }
    }

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly VerticalDbContext _dbContext;

        public RequestHandler(VerticalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var deposits = await _dbContext.Deposits
                .Include(x => x.Address)
                .Where(x => x.UserId == request.UserId)
                .Select(x => new DepositModel
                {
                    Id = x.Id,
                    CryptoAddress = x.Address.CryptoAddress,
                    Amount = x.Amount,
                    CurrencyCode = x.CurrencyCode,
                    TxId = x.TxId,
                    Confirmations = x.Confirmations,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .ToArrayAsync(cancellationToken);

            return new Response(deposits);
        }
    }
}
