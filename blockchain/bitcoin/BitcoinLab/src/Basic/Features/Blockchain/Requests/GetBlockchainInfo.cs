using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NBitcoin.RPC;

namespace Basic.Features.Blockchain.Requests;

public static class GetBlockchainInfo
{
    [HttpGet("/blockchain/info")]
    [AllowAnonymous]
    public class Endpoint : EndpointWithoutRequest
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator) => _mediator = mediator;

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new Request(), cancellationToken);
            await SendAsync(response, cancellation: cancellationToken);
        }
    }

    public record Request : IRequest<Response>;

    public record Response(ulong Blocks, ulong Difficulty, ulong Headers, string BestBlockHash, ulong SizeOnDisk, bool Pruned);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly RPCClient _client;

        public RequestHandler(RPCClient client)
        {
            _client = client;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var blockchainInfo = await _client.GetBlockchainInfoAsync(cancellationToken);

            return new(
                blockchainInfo.Blocks,
                blockchainInfo.Difficulty,
                blockchainInfo.Headers,
                blockchainInfo.BestBlockHash.ToString(),
                blockchainInfo.SizeOnDisk,
                blockchainInfo.Pruned);
        }
    }
}
