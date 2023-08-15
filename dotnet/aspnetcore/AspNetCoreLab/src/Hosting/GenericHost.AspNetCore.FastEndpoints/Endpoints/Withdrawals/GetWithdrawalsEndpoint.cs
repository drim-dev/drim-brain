using FastEndpoints;
using GenericHost.AspNetCore.FastEndpoints.Attributes;
using GenericHost.AspNetCore.FastEndpoints.Dtos;

namespace GenericHost.AspNetCore.FastEndpoints.Endpoints.Withdrawals;

public class GetWithdrawalsEndpoint : EndpointWithoutRequest<WithdrawalDto[]>
{
    public override void Configure()
    {
        Get("/withdrawals");
        AllowAnonymous();
        Options(x => x.WithMetadata(new RateLimitingAttribute(10_000)));
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
