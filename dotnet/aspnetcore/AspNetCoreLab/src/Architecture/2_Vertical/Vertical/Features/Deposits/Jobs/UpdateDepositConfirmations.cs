using MediatR;
using Services.Configuration;

namespace Vertical.Features.Deposits.Jobs;

public static class UpdateDepositConfirmations
{
    public class Job : IRequest<Unit>
    {
    }

    public class JobHandler : IRequestHandler<Job, Unit>
    {
        private readonly IDepositRepository _depositRepository;
        private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<JobHandler> _logger;

        public JobHandler(
            IDepositRepository depositRepository,
            IBitcoinBlockchainScanner bitcoinBlockchainScanner,
            IAccountRepository accountRepository,
            ILogger<JobHandler> logger)
        {
            _depositRepository = depositRepository;
            _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(Job request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deposit confirmations processing started");

            var unconfirmedDeposits = await _depositRepository.LoadUnconfirmedDeposits(cancellationToken);

            await _bitcoinBlockchainScanner.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

            await _depositRepository.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

            var confirmedDeposits = unconfirmedDeposits.Where(d => d.IsConfirmed);

            await _accountRepository.DepositToAccounts(confirmedDeposits, cancellationToken);

            _logger.LogInformation("Deposit confirmations processing finished");

            return Unit.Value;
        }
    }
}
