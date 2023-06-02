namespace Testing.Features.Users.Domain;

public class User
{
    public long Id { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public DateTime DateOfBirth { get; set; }

    public DateTime RegisteredAt { get; set; }
}
