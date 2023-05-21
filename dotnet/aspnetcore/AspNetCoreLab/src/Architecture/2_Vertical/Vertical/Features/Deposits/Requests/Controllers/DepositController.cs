using Microsoft.AspNetCore.Mvc;
using Vertical.Features.Deposits.Models;
using Vertical.Pipeline;

namespace Vertical.Features.Deposits.Requests.Controllers;

[ApiController]
[Route("/deposits")]
public class DepositController : Controller
{
    private readonly Dispatcher _dispatcher;

    public DepositController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<DepositModel[]> GetDeposits(int userId, CancellationToken cancellationToken)
    {
        var response = await _dispatcher.Dispatch(new GetDeposits.Request(userId), cancellationToken);
        return response.Deposits;
    }

    [HttpGet("address")]
    public async Task<DepositAddressModel> GetDepositAddress(int userId, CancellationToken cancellationToken)
    {
        var response = await _dispatcher.Dispatch(new GetDepositAddress.Request(userId), cancellationToken);
        return response.Address;
    }
}
