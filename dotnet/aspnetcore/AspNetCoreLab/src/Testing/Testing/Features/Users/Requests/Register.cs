using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Testing.Common.Passwords;
using Testing.Database;
using Testing.Features.Users.Domain;
using Testing.Features.Users.Options;
using static Testing.Features.Users.Errors.UsersValidationErrors;

namespace Testing.Features.Users.Requests;

public static class Register
{
    public record Request(string Email, string Password, DateTime DateOfBirth) : IRequest<Unit>;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(TestingDbContext db, IOptions<UsersOptions> options)
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(EmailRequired)
                .EmailAddress().WithErrorCode(InvalidEmailFormat)
                .MustAsync(async (email, ct) => !await db.Users.AnyAsync(x => x.Email == email.ToLower(), ct))
                    .WithErrorCode(EmailAlreadyTaken);
            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode(PasswordRequired)
                .MinimumLength(8).WithErrorCode(InvalidPasswordLength);
            RuleFor(x => x.DateOfBirth)
                .GreaterThan(DateTime.UtcNow.AddYears(-1 * options.Value.MaximumUserAgeInYears)).WithErrorCode(TooOld)
                .LessThan(DateTime.UtcNow.AddYears(-1 * options.Value.MinimumUserAgeInYears)).WithErrorCode(TooYoung);
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
