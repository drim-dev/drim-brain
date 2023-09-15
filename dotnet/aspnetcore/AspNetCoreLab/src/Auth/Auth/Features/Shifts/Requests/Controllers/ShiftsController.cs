using Auth.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Features.Shifts.Requests.Controllers;

[ApiController]
// [Authorize(Policy = PolicyNames.SellerFromRank3)]
[Route("/shifts")]
public class ShiftsController : Controller
{
    private readonly IMediator _mediator;

    public ShiftsController(IMediator mediator) => _mediator = mediator;

    [Authorize(Policy = PolicyNames.OpenShift)]
    [HttpPost("open")]
    public async Task<OpenShift.Response> OpenShift(CancellationToken cancellationToken) =>
        await _mediator.Send(new OpenShift.Request(), cancellationToken);

    [HttpPost("close")]
    public async Task<CloseShift.Response> CloseShift(CancellationToken cancellationToken) =>
        await _mediator.Send(new CloseShift.Request(), cancellationToken);
}
