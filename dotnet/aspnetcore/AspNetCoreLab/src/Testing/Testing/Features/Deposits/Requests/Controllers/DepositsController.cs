using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testing.Features.Auth.Extensions;

namespace Testing.Features.Deposits.Requests.Controllers;

[ApiController]
[Authorize]
[Route("/deposits")]
public class DepositsController : Controller
{
    private readonly IMediator _mediator;

    public DepositsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("address")]
    public async Task<GetDepositAddress.Response> GetDepositAddress(GetDepositAddressRequestModel request, CancellationToken cancellationToken) =>
        await _mediator.Send(new GetDepositAddress.Request(User.GetUserId(), request.Currency), cancellationToken);
}

public record GetDepositAddressRequestModel(string Currency);
