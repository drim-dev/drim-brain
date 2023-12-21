using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace EncryptionLab;

[MemoryDiagnoser]
public class Benchmark
{
    [Benchmark]
    public void SymmetricAes()
    {
        var plainTextBytes = "Hello, World!"u8.ToArray();
        var iv = RandomNumberGenerator.GetBytes(16);

        using var aes = Aes.Create();

        var encryptedTextBytes = aes.EncryptCbc(plainTextBytes, iv);
        aes.DecryptCbc(encryptedTextBytes, iv);
    }

    [Benchmark]
    public void AsymmetricRsa()
    {
        var plainTextBytes = "Hello, World!"u8.ToArray();

        using var rsa = RSA.Create();

        var encryptedTextBytes = rsa.Encrypt(plainTextBytes, RSAEncryptionPadding.OaepSHA256);
        rsa.Decrypt(encryptedTextBytes, RSAEncryptionPadding.OaepSHA256);
    }
}
