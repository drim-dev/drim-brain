using MediatR;

namespace BankingService.Features.Withdrawals.Requests;

internal class ListWithdrawals
{
    internal class RequestHandler : IRequestHandler<ListWithdrawalsRequest, ListWithdrawalsReply>
    {
        public Task<ListWithdrawalsReply> Handle(ListWithdrawalsRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ListWithdrawalsReply());
        }
    }
}
