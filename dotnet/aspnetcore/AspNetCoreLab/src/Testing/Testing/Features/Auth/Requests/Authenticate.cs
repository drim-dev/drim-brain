using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Testing.Common.Passwords;
using Testing.Database;
using Testing.Errors.Exceptions;
using Testing.Features.Auth.Options;

using static Testing.Features.Auth.Errors.AuthValidationErrors;

namespace Testing.Features.Auth.Requests;

public static class Authenticate
{
    public record Request(string Email, string Password) : IRequest<Response>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode(EmailRequired)
                .EmailAddress().WithErrorCode(InvalidEmailFormat);
            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode(PasswordRequired);
        }
    }

    public record Response(string Jwt);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly TestingDbContext _db;
        private readonly Argon2IdPasswordHasher _passwordHasher;
        private readonly AuthOptions _options;

        public RequestHandler(
            TestingDbContext db,
            Argon2IdPasswordHasher passwordHasher,
            IOptions<AuthOptions> options)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _options = options.Value;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == request.Email.ToLower(), cancellationToken);

            if (user is null || !_passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password))
            {
                throw new ValidationErrorsException(string.Empty, "Wrong email or password", WrongCredentials);
            }

            var jwt = GenerateJwt(user.Id);

            return new Response(jwt);
        }

        private string GenerateJwt(long id)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, id.ToString()),
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
