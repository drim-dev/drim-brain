using Grpc.Core;
using MediatR;

namespace BlockchainService.Features.Withdrawals;

public class WithdrawalsApi(IMediator _mediator) : BlockchainService.Withdrawals.WithdrawalsBase
{
    public override Task<WithdrawReply> Withdraw(WithdrawRequest request, ServerCallContext context) =>
        _mediator.Send(request, context.CancellationToken);
}
