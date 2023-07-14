using System.Data;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Transactions.Concurrency;
using Transactions.Database;
using Transactions.Errors;
using Transactions.Features.Accounts.Domain;
using Transactions.Features.Accounts.Errors;

namespace Transactions.Features.Accounts.Requests;

public static class TransferBetweenAccounts
{
    [AllowAnonymous]
    [HttpPost("/accounts/transfer")]
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator) => _mediator = mediator;

        public override Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }

    public record Request(int UserId, int FromAccountId, int ToAccountId, decimal Amount) : IRequest<Response>;

    public record Response(decimal FromAccountResultAmount, decimal ToAccountResultAmount);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var fromAccount = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.FromAccountId, cancellationToken);

            if (fromAccount is null)
            {
                throw new ValidationErrorsException(nameof(request.FromAccountId), "From account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            var toAccount = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.ToAccountId, cancellationToken);

            if (toAccount is null)
            {
                throw new ValidationErrorsException(nameof(request.ToAccountId), "To account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            if (fromAccount.Amount < request.Amount)
            {
                throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                    AccountValidationErrors.AccountAmountNotEnough);
            }

            fromAccount.Amount -= request.Amount;
            toAccount.Amount += request.Amount;

            await _db.SaveChangesAsync(cancellationToken);

            return new(fromAccount.Amount, toAccount.Amount);
        }
    }

    public class RequestHandlerMutex : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandlerMutex(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                Mutexes.AccountsMutex.WaitOne();

                var fromAccount = await _db.Accounts.SingleOrDefaultAsync(
                    x => x.UserId == request.UserId && x.Id == request.FromAccountId, cancellationToken);

                if (fromAccount is null)
                {
                    throw new ValidationErrorsException(nameof(request.FromAccountId), "From account not exists",
                        AccountValidationErrors.AccountNotExists);
                }

                var toAccount = await _db.Accounts.SingleOrDefaultAsync(
                    x => x.UserId == request.UserId && x.Id == request.ToAccountId, cancellationToken);

                if (toAccount is null)
                {
                    throw new ValidationErrorsException(nameof(request.ToAccountId), "To account not exists",
                        AccountValidationErrors.AccountNotExists);
                }

                if (fromAccount.Amount < request.Amount)
                {
                    throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                        AccountValidationErrors.AccountAmountNotEnough);
                }

                fromAccount.Amount -= request.Amount;
                toAccount.Amount += request.Amount;

                await _db.SaveChangesAsync(cancellationToken);

                return new(fromAccount.Amount, toAccount.Amount);
            }
            finally
            {
                Mutexes.AccountsMutex.ReleaseMutex();
            }
        }
    }

    public class RequestHandlerNamedMutex : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandlerNamedMutex(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                Mutexes.AccountsMutexNamed.WaitOne();

                var fromAccount = await _db.Accounts.SingleOrDefaultAsync(
                    x => x.UserId == request.UserId && x.Id == request.FromAccountId, cancellationToken);

                if (fromAccount is null)
                {
                    throw new ValidationErrorsException(nameof(request.FromAccountId), "From account not exists",
                        AccountValidationErrors.AccountNotExists);
                }

                var toAccount = await _db.Accounts.SingleOrDefaultAsync(
                    x => x.UserId == request.UserId && x.Id == request.ToAccountId, cancellationToken);

                if (toAccount is null)
                {
                    throw new ValidationErrorsException(nameof(request.ToAccountId), "To account not exists",
                        AccountValidationErrors.AccountNotExists);
                }

                if (fromAccount.Amount < request.Amount)
                {
                    throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                        AccountValidationErrors.AccountAmountNotEnough);
                }

                fromAccount.Amount -= request.Amount;
                toAccount.Amount += request.Amount;

                await _db.SaveChangesAsync(cancellationToken);

                return new(fromAccount.Amount, toAccount.Amount);
            }
            finally
            {
                Mutexes.AccountsMutexNamed.ReleaseMutex();
            }
        }
    }

    public class RequestHandlerDistributedLock : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;
        private readonly IRedisClient _redisClient;

        public RequestHandlerDistributedLock(
            AppDbContext db,
            IRedisClient redisClient)
        {
            _db = db;
            _redisClient = redisClient;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await using var redisLock = await _redisClient.ObtainLock("AccountsLock", cancellationToken);

            var fromAccount = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.FromAccountId, cancellationToken);

            if (fromAccount is null)
            {
                throw new ValidationErrorsException(nameof(request.FromAccountId), "From account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            var toAccount = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.ToAccountId, cancellationToken);

            if (toAccount is null)
            {
                throw new ValidationErrorsException(nameof(request.ToAccountId), "To account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            if (fromAccount.Amount < request.Amount)
            {
                throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                    AccountValidationErrors.AccountAmountNotEnough);
            }

            fromAccount.Amount -= request.Amount;
            toAccount.Amount += request.Amount;

            await _db.SaveChangesAsync(cancellationToken);

            return new(fromAccount.Amount, toAccount.Amount);
        }
    }

    public class RequestHandlerVersioned : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandlerVersioned(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            VersionedAccount? fromAccount = null;
            VersionedAccount? toAccount = null;
            var updated = false;

            for (var i = 0; i < 3; i++)
            {
                try
                {
                    fromAccount = await _db.VersionedAccounts.SingleOrDefaultAsync(
                        x => x.UserId == request.UserId && x.Id == request.FromAccountId, cancellationToken);

                    if (fromAccount is null)
                    {
                        throw new ValidationErrorsException(nameof(request.FromAccountId), "From account not exists",
                            AccountValidationErrors.AccountNotExists);
                    }

                    toAccount = await _db.VersionedAccounts.SingleOrDefaultAsync(
                        x => x.UserId == request.UserId && x.Id == request.ToAccountId, cancellationToken);

                    if (toAccount is null)
                    {
                        throw new ValidationErrorsException(nameof(request.ToAccountId), "To account not exists",
                            AccountValidationErrors.AccountNotExists);
                    }

                    if (fromAccount.Amount < request.Amount)
                    {
                        throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                            AccountValidationErrors.AccountAmountNotEnough);
                    }

                    fromAccount.Amount -= request.Amount;
                    toAccount.Amount += request.Amount;

                    await _db.SaveChangesAsync(cancellationToken);

                    updated = true;

                    break;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // continue
                }
            }

            if (!updated)
            {
                throw new Exception("Failed to update accounts");
            }

            return new(fromAccount!.Amount, toAccount!.Amount);
        }
    }

    public class RequestHandlerIsolationLevelRepeatableRead : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandlerIsolationLevelRepeatableRead(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead,
                cancellationToken);

            var fromAccount = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.FromAccountId, cancellationToken);

            if (fromAccount is null)
            {
                throw new ValidationErrorsException(nameof(request.FromAccountId), "From account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            var toAccount = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.ToAccountId, cancellationToken);

            if (toAccount is null)
            {
                throw new ValidationErrorsException(nameof(request.ToAccountId), "To account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            if (fromAccount.Amount < request.Amount)
            {
                throw new ValidationErrorsException(nameof(request.Amount), "Account amount not enough",
                    AccountValidationErrors.AccountAmountNotEnough);
            }

            fromAccount.Amount -= request.Amount;
            toAccount.Amount += request.Amount;

            await _db.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return new(fromAccount.Amount, toAccount.Amount);
        }
    }
}
