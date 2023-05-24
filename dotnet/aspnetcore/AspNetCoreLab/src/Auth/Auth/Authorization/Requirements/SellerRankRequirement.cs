using Auth.Features.Auth.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Authorization.Requirements;

public class SellerRankRequirement : IAuthorizationRequirement
{
    public SellerRankRequirement(int requiredRank)
    {
        RequiredRank = requiredRank;
    }

    public int RequiredRank { get; }
}

public class SellerRankRequirementHandler : AuthorizationHandler<SellerRankRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SellerRankRequirement requirement)
    {
        var rankClaim = context.User.FindFirst(CustomClaims.Rank);
        if (rankClaim is null)
        {
            return Task.CompletedTask;
        }

        var rank = int.Parse(rankClaim.Value);

        if (rank >= requirement.RequiredRank)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
