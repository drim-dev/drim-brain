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

namespace Transactions.Features.Deposits.Requests;

public static class DepositToAccount
{
    [AllowAnonymous]
    [HttpPost("/deposits")]
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMediator _mediator;

        public Endpoint(IMediator mediator) => _mediator = mediator;

        public override Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }

    public record Request(int UserId, int AccountId, decimal Amount) : IRequest<Response>;

    public record Response(decimal ResultAmount);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _db;

        public RequestHandler(AppDbContext db)
        {
            _db = db;
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

            account.Amount += request.Amount;

            await _db.SaveChangesAsync(cancellationToken);

            return new(account.Amount);
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

                var account = await _db.Accounts.SingleOrDefaultAsync(
                    x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

                if (account is null)
                {
                    throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                        AccountValidationErrors.AccountNotExists);
                }

                account.Amount += request.Amount;

                await _db.SaveChangesAsync(cancellationToken);

                return new(account.Amount);
            }
            finally
            {
                Mutexes.AccountsMutex.ReleaseMutex();
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

            var account = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

            if (account is null)
            {
                throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            account.Amount += request.Amount;

            await _db.SaveChangesAsync(cancellationToken);

            return new(account.Amount);
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
            VersionedAccount? account = null;
            var updated = false;

            for (var i = 0; i < 3; i++)
            {
                try
                {
                    account = await _db.VersionedAccounts.SingleOrDefaultAsync(
                        x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

                    if (account is null)
                    {
                        throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                            AccountValidationErrors.AccountNotExists);
                    }

                    account.Amount += request.Amount;

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
                throw new Exception("Failed to update account");
            }

            return new(account!.Amount);
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

            var account = await _db.Accounts.SingleOrDefaultAsync(
                x => x.UserId == request.UserId && x.Id == request.AccountId, cancellationToken);

            if (account is null)
            {
                throw new ValidationErrorsException(nameof(request.AccountId), "Account not exists",
                    AccountValidationErrors.AccountNotExists);
            }

            account.Amount += request.Amount;

            await _db.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return new(account.Amount);
        }
    }
}
