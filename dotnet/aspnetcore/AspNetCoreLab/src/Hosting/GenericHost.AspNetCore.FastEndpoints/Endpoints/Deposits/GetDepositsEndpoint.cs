using FastEndpoints;
using GenericHost.AspNetCore.FastEndpoints.Attributes;
using GenericHost.AspNetCore.FastEndpoints.Dtos;
using Microsoft.AspNetCore.Authorization;
using Services.Configuration;

namespace GenericHost.AspNetCore.FastEndpoints.Endpoints.Deposits;

public class GetDepositsEndpoint : EndpointWithoutRequest<DepositDto[]>
{
    private readonly IDepositRepository _depositRepository;

    public GetDepositsEndpoint(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public override void Configure()
    {
        Get("/deposits");
        AllowAnonymous();
        Options(x => x.WithMetadata(new RateLimitingAttribute(10_000)));
    }

    public override async Task<DepositDto[]> ExecuteAsync(CancellationToken cancellationToken) =>
        (await _depositRepository.LoadAllDeposits(cancellationToken))
            .Select(x => new DepositDto
            {
                UserId = x.UserId,
                Currency = x.Currency,
                Amount = x.Amount,
                IsConfirmed = x.IsConfirmed,
            })
            .ToArray();
}
