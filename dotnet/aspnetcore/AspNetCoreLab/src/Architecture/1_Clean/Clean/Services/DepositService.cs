using Clean.Database;
using Clean.Domain;
using Clean.Models;
using Clean.Repositories.Abstract;
using Clean.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Clean.Services;

public class DepositService : IDepositService
{
    private readonly IDepositRepository _depositRepository;
    private readonly IDepositAddressRepository _depositAddressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositService(
        IDepositRepository depositRepository,
        IDepositAddressRepository depositAddressRepository,
        IUnitOfWork unitOfWork)
    {
        _depositRepository = depositRepository;
        _depositAddressRepository = depositAddressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepositModel[]> GetDeposits(int userId, CancellationToken cancellationToken)
    {
        var deposits = await _depositRepository.GetDepositsByUserId(userId, cancellationToken);
        return deposits
            .Select(x => new DepositModel
            {
                Id = x.Id,
                CryptoAddress = x.Address.CryptoAddress,
                Amount = x.Amount,
                CurrencyCode = x.CurrencyCode,
                TxId = x.TxId,
                Confirmations = x.Confirmations,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .ToArray();
    }

    public async Task<DepositAddressModel> GetDepositAddress(int userId, CancellationToken cancellationToken)
    {
        var existingAddress = await _depositAddressRepository.GetDepositAddressByUserId(userId, cancellationToken);
        if (existingAddress is not null)
        {
            return new DepositAddressModel(existingAddress.CryptoAddress);
        }

        var address = new DepositAddress
        {
            UserId = userId,
            CurrencyCode = "BTC",
            CryptoAddress = GenerateAddress(userId),
            DerivationIndex = 0,
            XpubId = 1,
        };

        _depositAddressRepository.Add(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DepositAddressModel(address.CryptoAddress);
    }

    private string GenerateAddress(int userId)
    {
        return Guid.NewGuid().ToString();
    }
}
