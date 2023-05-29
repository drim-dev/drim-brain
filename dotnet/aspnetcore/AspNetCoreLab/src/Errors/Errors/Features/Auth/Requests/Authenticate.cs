using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Errors.Features.Auth.Options;
using Errors.Storages;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Errors.Features.Auth.Requests;

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

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!await UserStorage.Exists(request.Name, request.Password, cancellationToken))
            {
                throw new Exception("Invalid credentials");
            }

            var jwt = GenerateJwt(request.Name);

            return new Response(jwt);
        }

        private string GenerateJwt(string name)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, name),
            };

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
