using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Testing.Features.Users.Requests.Controllers;

[ApiController]
[Route("/users")]
public class UsersController : Controller
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous]
    [HttpPost]
    public async Task Register(Register.Request request, CancellationToken cancellationToken) =>
        await _mediator.Send(request, cancellationToken);
}
