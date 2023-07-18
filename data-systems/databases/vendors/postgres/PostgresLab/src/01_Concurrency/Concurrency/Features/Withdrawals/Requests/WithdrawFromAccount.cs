using System.Data;
using FastEndpoints;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Transactions.Database;
using Transactions.Errors;
using Transactions.Features.Accounts.Errors;
using Transactions.Features.Withdrawals.Domain;
using Transactions.Features.Withdrawals.Errors;
using Transactions.Features.Withdrawals.Options;
using Transactions.Features.Withdrawals.Services;

namespace Transactions.Features.Withdrawals.Requests;

public static class WithdrawFromAccount
{
    [HttpPost("/withdrawals")]
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator) => _mediator = mediator;

        public override Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }

    public record Request(int UserId, int AccountId, decimal Amount, string ToAddress) : IRequest<Response>;

    public record Response(decimal ResultAmount);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;
        private readonly CryptoSender _cryptoSender;
        private readonly WithdrawalsOptions _options;

        public RequestHandler(
            AppDbContext db,
            CryptoSender cryptoSender,
            IOptions<WithdrawalsOptions> options)
        {
            _db = db;
            _cryptoSender = cryptoSender;
            _options = options.Value;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

            if (account is null)
            {
                throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            if (account.Amount < request.Amount)
            {
                throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                    AccountValidationErrors.AccountAmountNotEnough);
            }

            account.Amount -= request.Amount;

            var withdrawal = new Withdrawal
            {
                UserId = request.UserId,
                AccountId = request.AccountId,
                CurrencyCode = account.CurrencyCode,
                Amount = request.Amount,
                ToAddress = request.ToAddress,
                CreatedAt = DateTime.UtcNow,
            };

            await ThrowIfDailyLimitExceeded(withdrawal, cancellationToken);

            _db.Withdrawals.Add(withdrawal);

            await _cryptoSender.Send(withdrawal, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return new(account.Amount);
        }

        private async Task ThrowIfDailyLimitExceeded(Withdrawal withdrawal, CancellationToken cancellationToken)
        {
            var amountWithdrawnToday = await _db.Withdrawals
                .Where(x => x.UserId == withdrawal.UserId && x.CreatedAt.Date == DateTime.UtcNow.Date)
                .SumAsync(x => x.Amount, cancellationToken);

            if (amountWithdrawnToday + withdrawal.Amount > _options.DailyLimit)
            {
                throw new LogicConflictException("Daily withdrawal limit exceeded",
                    WithdrawalLogicConflictErrors.AccountDailyWithdrawalLimitExceeded);
            }
        }
    }

    public class RequestHandlerRepeatableRead : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;
        private readonly CryptoSender _cryptoSender;
        private readonly WithdrawalsOptions _options;

        public RequestHandlerRepeatableRead(
            AppDbContext db,
            CryptoSender cryptoSender,
            IOptions<WithdrawalsOptions> options)
        {
            _db = db;
            _cryptoSender = cryptoSender;
            _options = options.Value;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

            var account = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

            if (account is null)
            {
                throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            if (account.Amount < request.Amount)
            {
                throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                    AccountValidationErrors.AccountAmountNotEnough);
            }

            account.Amount -= request.Amount;

            var withdrawal = new Withdrawal
            {
                UserId = request.UserId,
                AccountId = request.AccountId,
                CurrencyCode = account.CurrencyCode,
                Amount = request.Amount,
                ToAddress = request.ToAddress,
                CreatedAt = DateTime.UtcNow,
            };

            await ThrowIfDailyLimitExceeded(withdrawal, cancellationToken);

            _db.Withdrawals.Add(withdrawal);

            await _cryptoSender.Send(withdrawal, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return new(account.Amount);
        }

        private async Task ThrowIfDailyLimitExceeded(Withdrawal withdrawal, CancellationToken cancellationToken)
        {
            var amountWithdrawnToday = await _db.Withdrawals
                .Where(x => x.UserId == withdrawal.UserId && x.CreatedAt.Date == DateTime.UtcNow.Date)
                .SumAsync(x => x.Amount, cancellationToken);

            if (amountWithdrawnToday + withdrawal.Amount > _options.DailyLimit)
            {
                throw new LogicConflictException("Daily withdrawal limit exceeded",
                    WithdrawalLogicConflictErrors.AccountDailyWithdrawalLimitExceeded);
            }
        }
    }

    public class RequestHandlerSerializable : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;
        private readonly CryptoSender _cryptoSender;
        private readonly WithdrawalsOptions _options;

        public RequestHandlerSerializable(
            AppDbContext db,
            CryptoSender cryptoSender,
            IOptions<WithdrawalsOptions> options)
        {
            _db = db;
            _cryptoSender = cryptoSender;
            _options = options.Value;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var account = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

            if (account is null)
            {
                throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            if (account.Amount < request.Amount)
            {
                throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                    AccountValidationErrors.AccountAmountNotEnough);
            }

            account.Amount -= request.Amount;

            var withdrawal = new Withdrawal
            {
                UserId = request.UserId,
                AccountId = request.AccountId,
                CurrencyCode = account.CurrencyCode,
                Amount = request.Amount,
                ToAddress = request.ToAddress,
                CreatedAt = DateTime.UtcNow,
            };

            await ThrowIfDailyLimitExceeded(withdrawal, cancellationToken);

            _db.Withdrawals.Add(withdrawal);

            await _cryptoSender.Send(withdrawal, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return new(account.Amount);
        }

        private async Task ThrowIfDailyLimitExceeded(Withdrawal withdrawal, CancellationToken cancellationToken)
        {
            var amountWithdrawnToday = await _db.Withdrawals
                .Where(x => x.UserId == withdrawal.UserId && x.CreatedAt.Date == DateTime.UtcNow.Date)
                .SumAsync(x => x.Amount, cancellationToken);

            if (amountWithdrawnToday + withdrawal.Amount > _options.DailyLimit)
            {
                throw new LogicConflictException("Daily withdrawal limit exceeded",
                    WithdrawalLogicConflictErrors.AccountDailyWithdrawalLimitExceeded);
            }
        }
    }

    public class RequestHandlerSerializableOutbox : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;
        private readonly WithdrawalsOptions _options;

        public RequestHandlerSerializableOutbox(
            AppDbContext db,
            IOptions<WithdrawalsOptions> options)
        {
            _db = db;
            _options = options.Value;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var account = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

            if (account is null)
            {
                throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            if (account.Amount < request.Amount)
            {
                throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                    AccountValidationErrors.AccountAmountNotEnough);
            }

            account.Amount -= request.Amount;

            var withdrawal = new Withdrawal
            {
                UserId = request.UserId,
                AccountId = request.AccountId,
                CurrencyCode = account.CurrencyCode,
                Amount = request.Amount,
                ToAddress = request.ToAddress,
                CreatedAt = DateTime.UtcNow,
            };

            await ThrowIfDailyLimitExceeded(withdrawal, cancellationToken);

            _db.Withdrawals.Add(withdrawal);

            var outboxRecord = new WithdrawalOutboxRecord
            {
                Withdrawal = withdrawal,
                CreatedAt = DateTime.UtcNow,
                Status = WithdrawalOutboxRecordStatus.NotSent,
            };

            _db.WithdrawalOutboxRecords.Add(outboxRecord);

            await _db.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return new(account.Amount);
        }

        private async Task ThrowIfDailyLimitExceeded(Withdrawal withdrawal, CancellationToken cancellationToken)
        {
            var amountWithdrawnToday = await _db.Withdrawals
                .Where(x => x.UserId == withdrawal.UserId && x.CreatedAt.Date == DateTime.UtcNow.Date)
                .SumAsync(x => x.Amount, cancellationToken);

            if (amountWithdrawnToday + withdrawal.Amount > _options.DailyLimit)
            {
                throw new LogicConflictException("Daily withdrawal limit exceeded",
                    WithdrawalLogicConflictErrors.AccountDailyWithdrawalLimitExceeded);
            }
        }
    }
}
