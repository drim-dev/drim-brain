using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vertical.Database;
using Vertical.Features.Deposits.Domain;
using Vertical.Features.Deposits.Models;
using Vertical.Features.Deposits.Services;

namespace Vertical.Features.Deposits.Requests;

public static class GetDepositAddress
{
    public record Request(int UserId) : IRequest<Response>;

    public record Response(DepositAddressModel Address);

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
        private readonly VerticalDbContext _db;
        private readonly CryptoAddressGenerator _addressGenerator;

        public RequestHandler(
            VerticalDbContext db,
            CryptoAddressGenerator addressGenerator)
        {
            _db = db;
            _addressGenerator = addressGenerator;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var existingAddress = await _db.DepositAddresses
                .SingleOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

            if (existingAddress is not null)
            {
                return new Response(new(existingAddress.CryptoAddress));
            }

            var cryptoAddress = _addressGenerator.GenerateAddress();

            var address = new DepositAddress
            {
                UserId = request.UserId,
                CurrencyCode = "BTC",
                CryptoAddress = cryptoAddress,
                DerivationIndex = 1, // TODO
                XpubId = 1, // TODO
            };

            _db.DepositAddresses.Add(address);
            await _db.SaveChangesAsync(cancellationToken);

            return new Response(new(address.CryptoAddress));
        }
    }
}
