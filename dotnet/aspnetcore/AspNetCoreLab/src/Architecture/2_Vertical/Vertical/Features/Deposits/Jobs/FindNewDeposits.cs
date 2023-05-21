using MediatR;
using Services.Configuration;

namespace Vertical.Features.Deposits.Jobs;

public class FindNewDeposits
{
    public class Job : IRequest<Unit>
    {
    }

    public class JobHandler : IRequestHandler<Job, Unit>
    {
        private readonly IDepositAddressRepository _depositAddressRepository;
        private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
        private readonly IDepositRepository _depositRepository;
        private readonly ILogger<JobHandler> _logger;

        public JobHandler(
            IDepositAddressRepository depositAddressRepository,
            IBitcoinBlockchainScanner bitcoinBlockchainScanner,
            IDepositRepository depositRepository,
            ILogger<JobHandler> logger)
        {
            _depositAddressRepository = depositAddressRepository;
            _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
            _depositRepository = depositRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(Job request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("New deposit processing started");

            var depositAddresses = await _depositAddressRepository.LoadDepositAddresses(cancellationToken);

            var deposits = await _bitcoinBlockchainScanner.FindNewDeposits(depositAddresses, cancellationToken);

            await _depositRepository.SaveNewDeposits(deposits, cancellationToken);

            _logger.LogInformation("New deposit processing finished");

            return Unit.Value;
        }
    }
}
