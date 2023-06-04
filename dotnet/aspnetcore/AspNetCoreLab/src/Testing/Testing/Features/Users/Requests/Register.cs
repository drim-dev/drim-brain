using FluentValidation;
using MediatR;
using Testing.Common.Passwords;
using Testing.Database;
using Testing.Features.Users.Domain;

namespace Testing.Features.Users.Requests;

public static class Register
{
    public record Request(string Email, string Password, DateTime DateOfBirth) : IRequest<Unit>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }

    public class RequestHandler : IRequestHandler<Request, Unit>
    {
        private readonly TestingDbContext _db;
        private readonly Argon2IdPasswordHasher _passwordHasher;

        public RequestHandler(
            TestingDbContext db,
            Argon2IdPasswordHasher passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = passwordHash,
                DateOfBirth = request.DateOfBirth.ToUniversalTime(),
                RegisteredAt = DateTime.UtcNow,
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
