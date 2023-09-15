using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vertical.Database;
using Vertical.Features.Deposits.Models;
using Vertical.Pipeline;

namespace Vertical.Features.Deposits.Requests;

public static class GetDeposits
{
    [HttpGet("/deposits")]
    [AllowAnonymous]
    public class Endpoint : Endpoint<Request, DepositModel[]>
    {
        private readonly VerticalDbContext _dbContext;

        public Endpoint(VerticalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<DepositModel[]> ExecuteAsync(Request request, CancellationToken cancellationToken) =>
            await _dbContext.Deposits
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
    }

    [HttpGet("/deposits")]
    [AllowAnonymous]
    public class MediatorEndpoint : Endpoint<Request, DepositModel[]>
    {
        private readonly IMediator _mediator;

        public MediatorEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<DepositModel[]> ExecuteAsync(Request request, CancellationToken cancellationToken) =>
            await _mediator.Send(request, cancellationToken);
    }

    // TODO: add filtering, sorting and pagination
    public record Request(int UserId) : IRequest<DepositModel[]>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(0);
        }
    }

    public class RequestHandler : IRequestHandler<Request, DepositModel[]>
    {
        private readonly VerticalDbContext _dbContext;

        public RequestHandler(VerticalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DepositModel[]> Handle(Request request, CancellationToken cancellationToken) =>
            await _dbContext.Deposits
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
    }
}
