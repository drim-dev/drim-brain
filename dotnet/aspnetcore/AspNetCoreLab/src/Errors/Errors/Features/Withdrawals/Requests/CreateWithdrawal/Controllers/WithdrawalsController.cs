﻿using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Errors.Features.Withdrawals.Requests.CreateWithdrawal.Controllers;

[ApiController]
[Route("/withdrawals")]
public class WithdrawalsController : Controller
{
    private readonly IMediator _mediator;

    public WithdrawalsController(IMediator mediator) => _mediator = mediator;

    [Authorize]
    [HttpPost("basic-validation")]
    public async Task CreateWithdrawalBasicValidation(CreateWithdrawalBasicValidation.Request request, CancellationToken cancellationToken)
    {
        var userId = User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

        request.UserName = userId;

        await _mediator.Send(request, cancellationToken);
    }

    [Authorize]
    [HttpPost("codes-validation")]
    public async Task CreateWithdrawalCodesValidation(CreateWithdrawalCodesValidation.Request request, CancellationToken cancellationToken)
    {
        var userId = User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

        request.UserName = userId;

        await _mediator.Send(request, cancellationToken);
    }

    [Authorize]
    [HttpPost("async-validation")]
    public async Task CreateWithdrawalAsyncValidation(CreateWithdrawalAsyncValidation.Request request, CancellationToken cancellationToken)
    {
        var userId = User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

        request.UserName = userId;

        await _mediator.Send(request, cancellationToken);
    }

    [Authorize]
    [HttpPost("logic-conflict")]
    public async Task CreateWithdrawalLogicConflict(CreateWithdrawalLogicConflict.Request request, CancellationToken cancellationToken)
    {
        var userId = User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

        request.UserName = userId;

        await _mediator.Send(request, cancellationToken);
    }

    [Authorize]
    [HttpPost("complete")]
    public async Task CreateWithdrawalComplete(CreateWithdrawalComplete.Request request, CancellationToken cancellationToken)
    {
        var userId = User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

        request.UserName = userId;

        await _mediator.Send(request, cancellationToken);
    }
}
