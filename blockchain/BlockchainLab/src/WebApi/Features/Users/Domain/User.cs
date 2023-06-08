namespace WebApi.Features.Users.Domain;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime RegisteredAt { get; set; }
}
