namespace Testing.Common.Passwords;

public class Argon2IdOptions
{
    public int PasswordHashSizeInBytes { get; set; }

    public int SaltSizeInBytes { get; set; }

    public int DegreeOfParallelism { get; set; }

    public int MemorySize { get; set; }

    public int Iterations { get; set; }
}
