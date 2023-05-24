using Auth.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Features.Payments.Requests.Controllers;

[ApiController]
[Route("/payments")]
public class PaymentsController : Controller
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [Authorize(Policy = PolicyNames.SellerRole)]
    [HttpGet("generate")]
    public async Task<GenerateQrCode.Response> GenerateQrCode(decimal amount, CancellationToken cancellationToken) =>
        await _mediator.Send(new GenerateQrCode.Request(amount), cancellationToken);

    [Authorize(Policy = PolicyNames.BuyerRole)]
    [HttpPost("pay")]
    public async Task<PayWithQrCode.Response> PayWithQrCode(PayWithQrCode.Request request, CancellationToken cancellationToken) =>
        await _mediator.Send(request, cancellationToken);
}
