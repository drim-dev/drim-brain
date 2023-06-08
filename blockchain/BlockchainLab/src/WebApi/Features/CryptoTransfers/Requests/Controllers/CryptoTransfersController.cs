using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Features.CryptoTransfers.Requests.Controllers;

[ApiController]
[Route("CryptoTransfers")]
public class CryptoTransfersController : Controller
{
    private readonly IMediator _mediator;

    public CryptoTransfersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("deposit-address")]
    public async Task<GetDepositAddress.Response> GetDepositAddress([FromQuery] string currencyCode,
        CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.First(x => x.Type == "UserId").Value;
        var userId = Guid.Parse(userIdString);

        return await _mediator.Send(new GetDepositAddress.Request(userId, currencyCode), cancellationToken);
    }

    // [HttpGet]
    // public async Task<GetDepositAddress.Response> GetDepositAddress(GetDepositAddress.Request request,
    //     CancellationToken cancellationToken)
    // {
    //     return await _mediator.Send(request, cancellationToken);
    // }
}
