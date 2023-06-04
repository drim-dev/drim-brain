using FluentAssertions;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using Testing.Common.Passwords;

namespace Testing.Tests.Unit.Common.Passwords;

public class Argon2IdPasswordHasherTests
{
    [Fact]
    public void HashPassword_should_embed_parameters_salt_and_hash_into_result()
    {
        // Arrange
        var options = new Argon2IdOptions
        {
            PasswordHashSizeInBytes = 32,
            SaltSizeInBytes = 16,
            DegreeOfParallelism = 4,
            MemorySize = 4096,
            Iterations = 3,
        };

        var hasher = new Argon2IdPasswordHasher(Options.Create(options));

        // Act
        var passwordHash = hasher.HashPassword("test_password");

        // Assert
        passwordHash.Should().NotBeNullOrWhiteSpace();
        var parts = passwordHash.Split('$');
        parts[0].Should().Be("argon2id");
        parts[1].Should().Be($"m={options.MemorySize}");
        parts[2].Should().Be($"i={options.Iterations}");
        parts[3].Should().Be($"p={options.DegreeOfParallelism}");

        var saltBytes = Convert.FromBase64String(parts[4]);
        saltBytes.Should().HaveCount(options.SaltSizeInBytes);

        var hashBytes = Convert.FromBase64String(parts[5]);
        hashBytes.Should().HaveCount(options.PasswordHashSizeInBytes);

        using var argon2Id = new Argon2id("test_password"u8.ToArray())
        {
            Salt = saltBytes,
            DegreeOfParallelism = options.DegreeOfParallelism,
            MemorySize = options.MemorySize,
            Iterations = options.Iterations,
        };
        var expectedHashBytes = argon2Id.GetBytes(options.PasswordHashSizeInBytes);
        hashBytes.Should().BeEquivalentTo(expectedHashBytes);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_true_for_correct_hash()
    {
        // Arrange
        var hash = "argon2id$m=4096$i=3$p=4$HKPdgfRQ14YaszwdAWFpew==$mgzo7Z33PQbH+oyJwJiYCD9tMYxL+z1FCsLFx99ZNvY=";

        var hasher = new Argon2IdPasswordHasher(Options.Create<Argon2IdOptions>(new()));

        // Act
        var result = hasher.VerifyHashedPassword(hash, "test_password");

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    // Changing each parameter should result in a different hash
    [InlineData("argon2id$m=5096$i=3$p=4$HKPdgfRQ14YaszwdAWFpew==$mgzo7Z33PQbH+oyJwJiYCD9tMYxL+z1FCsLFx99ZNvY=")]
    [InlineData("argon2id$m=4096$i=4$p=4$HKPdgfRQ14YaszwdAWFpew==$mgzo7Z33PQbH+oyJwJiYCD9tMYxL+z1FCsLFx99ZNvY=")]
    [InlineData("argon2id$m=4096$i=3$p=5$HKPdgfRQ14YaszwdAWFpew==$mgzo7Z33PQbH+oyJwJiYCD9tMYxL+z1FCsLFx99ZNvY=")]
    [InlineData("argon2id$m=4096$i=3$p=4$IKPdgfRQ14YaszwdAWFpew==$mgzo7Z33PQbH+oyJwJiYCD9tMYxL+z1FCsLFx99ZNvY=")]
    [InlineData("argon2id$m=4096$i=3$p=4$HKPdgfRQ14YaszwdAWFpew==$ngzo7Z33PQbH+oyJwJiYCD9tMYxL+z1FCsLFx99ZNvY=")]
    public void VerifyHashedPassword_should_return_false_if_parameters_or_salt_or_hash_modified(string hash)
    {
        // Arrange
        var hasher = new Argon2IdPasswordHasher(Options.Create<Argon2IdOptions>(new()));

        // Act
        var result = hasher.VerifyHashedPassword(hash, "test_password");

        // Assert
        result.Should().BeFalse();
    }
}
