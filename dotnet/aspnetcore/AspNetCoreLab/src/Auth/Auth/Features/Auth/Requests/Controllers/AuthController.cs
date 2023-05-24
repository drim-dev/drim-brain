using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Features.Auth.Requests.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : Controller
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous]
    [HttpPost]
    public Task<Authenticate.Response> Authenticate(Authenticate.Request request, CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);

    [Authorize]
    [HttpGet("info")]
    public Task<GetUserInfo.Response> GetUserInfo(CancellationToken cancellationToken) =>
        _mediator.Send(new GetUserInfo.Request(User), cancellationToken);
}
