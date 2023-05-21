using Clean.Models;
using Clean.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Clean.Controllers;

[ApiController]
[Route("/deposits")]
public class DepositController : Controller
{
    private readonly IDepositService _depositService;

    public DepositController(IDepositService depositService)
    {
        _depositService = depositService;
    }

    [HttpGet]
    public async Task<DepositModel[]> GetDeposits(int userId, CancellationToken cancellationToken)
    {
        return await _depositService.GetDeposits(userId, cancellationToken);
    }

    [HttpGet("address")]
    public async Task<DepositAddressModel> GetDepositAddress(int userId, CancellationToken cancellationToken)
    {
        return await _depositService.GetDepositAddress(userId, cancellationToken);
    }
}
