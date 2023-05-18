using GenericHost.AspNetCore.Controllers.Attributes;
using GenericHost.AspNetCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Services.Configuration;

namespace GenericHost.AspNetCore.Controllers;

[Route("/deposits")]
public class DepositsController : Controller
{
    private readonly IDepositRepository _depositRepository;

    public DepositsController(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    [HttpGet]
    [RateLimiting(10_000)]
    public async Task<DepositDto[]> GetDeposits(CancellationToken cancellationToken) =>
        (await _depositRepository.LoadAllDeposits(cancellationToken))
            .Select(x => new DepositDto
            {
                UserId = x.UserId,
                Currency = x.Currency,
                Amount = x.Amount,
                IsConfirmed = x.IsConfirmed,
            })
            .ToArray();
}
