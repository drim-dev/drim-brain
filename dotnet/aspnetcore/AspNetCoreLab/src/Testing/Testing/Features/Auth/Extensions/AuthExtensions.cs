using System.Security.Claims;

namespace Testing.Features.Auth.Extensions;

public static class AuthExtensions
{
    public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            // TODO: use a custom exception
            throw new InvalidOperationException();
        }

        if (!int.TryParse(userId, out var parsedUserId))
        {
            // TODO: use a custom exception
            throw new InvalidOperationException();
        }

        return parsedUserId;
    }
}
