using System.Security.Claims;
using Auth.Features.Auth.Domain;
using FluentValidation;
using MediatR;

namespace Auth.Features.Auth.Requests;

public static class GetUserInfo
{
    public record Request(ClaimsPrincipal Principal) : IRequest<Response>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Principal).NotNull();
        }
    }

    public record Response(string? Name, string[] Roles, int? Rank);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var nameString = request.Principal.FindFirst(ClaimTypes.Name)?.Value ?? null;

            var roles = request.Principal.FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .ToArray();

            var rankString = request.Principal.FindFirst(CustomClaims.Rank)?.Value ?? null;
            int? rank = rankString is not null ? int.Parse(rankString) : null;

            return Task.FromResult(new Response(nameString, roles, rank));
        }
    }
}
