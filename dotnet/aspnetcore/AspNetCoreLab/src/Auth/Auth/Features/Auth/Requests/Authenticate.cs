using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Features.Auth.Domain;
using Auth.Features.Auth.Options;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Features.Auth.Requests;

public static class Authenticate
{
    public record Request(string Name, string Password) : IRequest<Response>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            // TODO: password strength
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public record Response(string Jwt);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AuthOptions _options;

        public RequestHandler(IOptions<AuthOptions> options)
        {
            _options = options.Value;
        }

        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var jwt = request switch
            {
                { Name: "buyer", Password: "buyer123456" } => GenerateJwt(request.Name, new [] { UserRole.Buyer }),
                { Name: "seller", Password: "seller123456" } => GenerateJwt(request.Name, new [] { UserRole.Seller, UserRole.Buyer }),
                { Name: "seller2", Password: "seller123456" } => GenerateJwt(request.Name, new [] { UserRole.Seller, UserRole.Buyer }, rank: 2),
                { Name: "seller5", Password: "seller123456" } => GenerateJwt(request.Name, new [] { UserRole.Seller, UserRole.Buyer }, rank: 5),
                { Name: "owner", Password: "owner123456" } => GenerateJwt(request.Name, new [] { UserRole.Owner, UserRole.Buyer }),
                _ => throw new Exception("Invalid credentials")
            };

            return Task.FromResult(new Response(jwt));
        }

        private string GenerateJwt(string name, UserRole[] roles, int? rank = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name),
            };

            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role.ToString()));
            }

            if (rank is not null)
            {
                claims.Add(new(CustomClaims.Rank, rank.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Jwt.SigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now + _options.Jwt.Expiration;

            var token = new JwtSecurityToken(
                _options.Jwt.Issuer,
                _options.Jwt.Audience,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
