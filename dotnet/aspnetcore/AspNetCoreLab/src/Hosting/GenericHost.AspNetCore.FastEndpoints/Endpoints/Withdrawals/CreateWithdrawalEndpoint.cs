using FastEndpoints;
using GenericHost.AspNetCore.FastEndpoints.Attributes;
using GenericHost.AspNetCore.FastEndpoints.Dtos;

namespace GenericHost.AspNetCore.FastEndpoints.Endpoints.Withdrawals;

public class CreateWithdrawalEndpoint : Endpoint<CreateWithrawalRequestDto, WithdrawalDto>
{
    public override void Configure()
    {
        Post("/withdrawals");
        AllowAnonymous();
        Options(x => x.WithMetadata(new RateLimitingAttribute(10_000)));
    }

    public override Task HandleAsync(CreateWithrawalRequestDto req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

public record CreateWithrawalRequestDto(int UserId, string Currency, decimal Amount, string ToAddress);