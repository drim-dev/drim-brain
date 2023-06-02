using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Testing.Common.Passwords;

public class Argon2IdPasswordHasher
{
    private readonly Argon2IdOptions _options;

    public Argon2IdPasswordHasher(IOptions<Argon2IdOptions> options)
    {
        _options = options.Value;
    }

    public string HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(_options.SaltSizeInBytes);

        using var argon2Id = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = saltBytes,
            DegreeOfParallelism = _options.DegreeOfParallelism,
            MemorySize = _options.MemorySize,
            Iterations = _options.Iterations,
        };

        var passwordHashBytes = argon2Id.GetBytes(_options.PasswordHashSizeInBytes);

        var saltInBase64 = Convert.ToBase64String(saltBytes);
        var passwordHashInBase64 = Convert.ToBase64String(passwordHashBytes);

        return $"argon2id$m={_options.MemorySize}$i={_options.Iterations}$p={_options.DegreeOfParallelism}${saltInBase64}${passwordHashInBase64}";
    }

    public bool VerifyHashedPassword(string passwordHash, string password)
    {
        if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var passwordHashParts = passwordHash.Split('$');

        if (passwordHashParts.Length != 6)
        {
            return false;
        }

        if (!int.TryParse(passwordHashParts[1].Split('=')[1], out var memorySize) ||
            !int.TryParse(passwordHashParts[2].Split('=')[1], out var iterations) ||
            !int.TryParse(passwordHashParts[3].Split('=')[1], out var degreeOfParallelism))
        {
            return false;
        }

        var saltBytes = Convert.FromBase64String(passwordHashParts[4]);
        var passwordHashBytes = Convert.FromBase64String(passwordHashParts[5]);

        using var argon2Id = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = saltBytes,
            MemorySize = memorySize,
            Iterations = iterations,
            DegreeOfParallelism = degreeOfParallelism,
        };

        var passwordHashBytesToCompare = argon2Id.GetBytes(passwordHashBytes.Length);

        return passwordHashBytesToCompare.SequenceEqual(passwordHashBytes);
    }
}
