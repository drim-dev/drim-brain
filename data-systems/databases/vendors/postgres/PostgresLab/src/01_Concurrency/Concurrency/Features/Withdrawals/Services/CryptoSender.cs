using Transactions.Features.Withdrawals.Domain;

namespace Transactions.Features.Withdrawals.Services;

public class CryptoSender
{
    public Task Send(Withdrawal withdrawal, CancellationToken cancellationToken)
    {
        // TODO: Send crypto using NBitcoin RpcClient
        return Task.CompletedTask;
    }
}
