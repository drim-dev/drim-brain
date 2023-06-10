using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using WebApi.Database;
using WebApi.Features.CryptoTransfers.Domain;

using static WebApi.Features.CryptoTransfers.Errors.GetDepositAddressValidationErrors;

namespace WebApi.Features.CryptoTransfers.Requests
{
    public static class GetDepositAddress
    {
        public record Request(Guid UserId, string CurrencyCode) : IRequest<Response>;

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(BlockchainDbContext db)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.UserId)
                    .NotEmpty().WithErrorCode(UserIdRequired)
                    .MustAsync(async (id, ct) =>
                    {
                        return await db.Users.AnyAsync(x => x.Id == id);
                    });
                RuleFor(x => x.CurrencyCode)
                    .NotEmpty().WithErrorCode(CurrencyCodeRequired)
                    .MustAsync(async (code, ct) =>
                    {
                        return await db.Currencies.AnyAsync(x => x.Code == code);
                    });
            }
        }

        public record Response(string CryptoAddress);

        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly BlockchainDbContext _db;
            private readonly IConfiguration _configuration;

            public RequestHandler(BlockchainDbContext db, IConfiguration configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var currency = _db.Currencies.FirstOrDefault(x => x.Code == request.CurrencyCode);



                var existingAddress = (await _db.DepositAddresses.ToListAsync(cancellationToken))
                    .FirstOrDefault(x=>x.UserId==request.UserId && x.CurrencyId==currency.Id);

                if (existingAddress is not null)
                {
                    return new Response(existingAddress.CryptoAddress);
                }

                var xpub = await _db.Xpubs
                    .Where(x => x.CurrencyId == currency.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                var variable = await _db.Variables.FirstOrDefaultAsync(x => x.Key == "BTC:DerivationIndex");
                var derivationIndexString = variable.Value;
                if (derivationIndexString is null)
                {
                    derivationIndexString = "-1";
                }


                if (!int.TryParse(derivationIndexString, out var derivationIndex))
                {
                    throw new Exception("Invalid derivation index");
                }
                derivationIndex++;

                variable.Value = derivationIndex.ToString();
                _db.Variables.Update(variable);
                _db.SaveChanges();

                var depositCryptoAddress = await GenerateCryptoAddress(xpub, derivationIndex);
                var depositAddress = new DepositAddress(currency.Id, request.UserId, xpub.Id, derivationIndex, depositCryptoAddress);
                _db.DepositAddresses.AddAsync(depositAddress);
                await _db.SaveChangesAsync(cancellationToken);

                return new Response(depositAddress.CryptoAddress);
            }
            private async Task<string> GenerateCryptoAddress(Xpub? xpub, int derivationIndex)
            {
                var networkConfig = _configuration.GetSection("BitcoinNetwork").Value;
                var network = Network.Main;
                if (networkConfig == "Testnet")
                {
                    network = Network.TestNet;
                }

                var extPubKey = ExtPubKey.Parse(xpub.Value, network)
                    .Derive(0, false);
                var derivedPubKey = extPubKey.Derive(derivationIndex, false).PubKey;

                var depositCryptoAddress = derivedPubKey.GetAddress(ScriptPubKeyType.Segwit, network).ToString();
                return depositCryptoAddress;
            }
        }
    }
}
