using Auth.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Features.Reporting.Requests.Controllers;

[ApiController]
[Authorize(Policy = PolicyNames.OwnerRole)]
[Route("/reporting")]
public class ReportingController : Controller
{
    private readonly IMediator _mediator;

    public ReportingController(IMediator mediator) => _mediator = mediator;

    [HttpGet("daily")]
    public async Task<GenerateDailyReport.Response> GenerateDailyReport(CancellationToken cancellationToken) =>
        await _mediator.Send(new GenerateDailyReport.Request(), cancellationToken);
}
