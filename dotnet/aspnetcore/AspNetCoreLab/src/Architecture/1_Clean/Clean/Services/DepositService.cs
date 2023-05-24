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
    private readonly CleanDbContext _db;

    public DepositService(
        IDepositRepository depositRepository,
        IDepositAddressRepository depositAddressRepository,
        IUnitOfWork unitOfWork,
        CleanDbContext db)
    {
        _depositRepository = depositRepository;
        _depositAddressRepository = depositAddressRepository;
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<DepositModel[]> GetDeposits(int userId, CancellationToken cancellationToken) =>
        await _db.Deposits
            .Include(x => x.Address)
            .Where(x => x.UserId == userId)
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
            .ToArrayAsync(cancellationToken);

    // var deposits = await _depositRepository.GetDepositsByUserId(userId, cancellationToken);
    // return deposits
    //     .Select(x => new DepositModel
    //     {
    //         Id = x.Id,
    //         CryptoAddress = x.Address.CryptoAddress,
    //         Amount = x.Amount,
    //         CurrencyCode = x.CurrencyCode,
    //         TxId = x.TxId,
    //         Confirmations = x.Confirmations,
    //         Status = x.Status,
    //         CreatedAt = x.CreatedAt,
    //         UpdatedAt = x.UpdatedAt,
    //     })
    //     .ToArray();

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
