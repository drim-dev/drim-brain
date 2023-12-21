using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace EncryptionLab;

[MemoryDiagnoser]
public class Benchmark
{
    private byte[]? _plainTextBytes;
    private byte[]? _iv;
    private Aes? _aes;
    private RSA? _rsa;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _plainTextBytes = "Hello, World!"u8.ToArray();
        _iv = RandomNumberGenerator.GetBytes(16);

        _aes = Aes.Create();
        _rsa = RSA.Create();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _aes!.Dispose();
        _rsa!.Dispose();
    }

    [Benchmark]
    public void SymmetricAes()
    {
        var encryptedTextBytes = _aes!.EncryptCbc(_plainTextBytes!, _iv!);
        _aes.DecryptCbc(encryptedTextBytes, _iv!);
    }

    [Benchmark]
    public void AsymmetricRsa()
    {
        var encryptedTextBytes = _rsa!.Encrypt(_plainTextBytes!, RSAEncryptionPadding.OaepSHA256);
        _rsa.Decrypt(encryptedTextBytes, RSAEncryptionPadding.OaepSHA256);
    }
}
