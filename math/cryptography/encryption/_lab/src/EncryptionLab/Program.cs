// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Running;
using EncryptionLab;

BenchmarkRunner.Run<Benchmark>();

// SymmetricAes();
// AsymmetricRsa();

void SymmetricAes()
{
    Console.Write("Enter text to encrypt with AES: ");
    var plainText = Console.ReadLine()!;
    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

    var key = RandomNumberGenerator.GetBytes(32);
    var iv = RandomNumberGenerator.GetBytes(16);

    using var aes = Aes.Create();
    aes.Key = key;

    Console.WriteLine($"\nKey: {Convert.ToBase64String(key)}\n");

    var encryptedTextBytes = aes.EncryptCbc(plainTextBytes, iv);
    var encryptedTextBase64 = Convert.ToBase64String(encryptedTextBytes);
    Console.WriteLine($"Encrypted text: {encryptedTextBase64}\n");

    var decryptedTextBytes = aes.DecryptCbc(encryptedTextBytes, iv);
    var decryptedText = Encoding.UTF8.GetString(decryptedTextBytes);
    Console.WriteLine($"Decrypted text: {decryptedText}\n");

    var wrongKey = RandomNumberGenerator.GetBytes(32);
    aes.Key = wrongKey;
    try
    {
        aes.DecryptCbc(encryptedTextBytes, iv);
    }
    catch (CryptographicException ex)
    {
        Console.WriteLine($"Decryption failed with wrong key\nException: {ex.Message}\n\n");
    }
}

void AsymmetricRsa()
{
    Console.Write("Enter text to encrypt with RSA: ");
    var plainText = Console.ReadLine()!;
    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

    using var keySource = RSA.Create();

    using var rsa = RSA.Create();

    var privateKey = keySource.ExportRSAPrivateKey();
    Console.WriteLine($"\nPrivate key: {Convert.ToBase64String(privateKey)}\n");

    var publicKey = keySource.ExportRSAPublicKey();
    Console.WriteLine($"Public key: {Convert.ToBase64String(publicKey)}\n");

    var encryptedTextBytes = rsa.Encrypt(plainTextBytes, RSAEncryptionPadding.OaepSHA256);
    var encryptedTextBase64 = Convert.ToBase64String(encryptedTextBytes);
    Console.WriteLine($"Encrypted text: {encryptedTextBase64}\n");

    var decryptedTextBytes = rsa.Decrypt(encryptedTextBytes, RSAEncryptionPadding.OaepSHA256);
    var decryptedText = Encoding.UTF8.GetString(decryptedTextBytes);
    Console.WriteLine($"Decrypted text: {decryptedText}\n");

    var wrongKeySource = RSA.Create();
    var wrongPrivateKey = wrongKeySource.ExportRSAPrivateKey();
    rsa.ImportRSAPrivateKey(wrongPrivateKey, out _);

    try
    {
        rsa.Decrypt(encryptedTextBytes, RSAEncryptionPadding.OaepSHA256);
    }
    catch (CryptographicException ex)
    {
        Console.WriteLine($"Decryption failed with wrong key\nException: {ex.Message}");
    }

    Console.WriteLine();
}
