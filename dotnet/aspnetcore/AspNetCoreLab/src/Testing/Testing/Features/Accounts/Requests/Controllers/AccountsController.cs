using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testing.Features.Auth.Extensions;

namespace Testing.Features.Accounts.Requests.Controllers;

[ApiController]
[Route("/accounts")]
public class AccountsController : Controller
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    public Task<GetAccounts.Response> GetAccounts(CancellationToken cancellationToken) =>
        _mediator.Send(new GetAccounts.Request(User.GetUserId()), cancellationToken);
}
