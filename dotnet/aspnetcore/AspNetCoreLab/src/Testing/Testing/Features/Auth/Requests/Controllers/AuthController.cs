using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Testing.Features.Auth.Requests.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : Controller
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public Task<Authenticate.Response> Authenticate(Authenticate.Request request, CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);
}
