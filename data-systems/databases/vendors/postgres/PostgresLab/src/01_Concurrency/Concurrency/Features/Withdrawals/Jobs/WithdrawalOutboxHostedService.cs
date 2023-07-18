using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Transactions.Database;
using Transactions.Features.Withdrawals.Domain;
using Transactions.Features.Withdrawals.Options;
using Transactions.Features.Withdrawals.Services;

namespace Transactions.Features.Withdrawals.Jobs;

public class WithdrawalOutboxHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public WithdrawalOutboxHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var options = scope.ServiceProvider.GetRequiredService<IOptions<WithdrawalsOptions>>().Value;
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cryptoSender = scope.ServiceProvider.GetRequiredService<CryptoSender>();

            var records = await db.WithdrawalOutboxRecords
                .Include(x => x.Withdrawal)
                .Where(x => x.Status == WithdrawalOutboxRecordStatus.NotSent)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(stoppingToken);

            foreach (var record in records)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(options.OutboxRecordProcessingTimeout);

                try
                {
                    await cryptoSender.Send(record.Withdrawal, cts.Token);
                    record.Status = WithdrawalOutboxRecordStatus.Sent;
                }
                catch (Exception ex)
                {
                    // TODO: examine exception and decide can withdrawal be sent again or should be marked as ManualReview
                    record.Status = WithdrawalOutboxRecordStatus.ManualReview;
                }

                await db.SaveChangesAsync(cts.Token);
            }

            await Task.Delay(options.OutboxProcessingInterval, stoppingToken);
        }
    }
}
