// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Security.Cryptography;
using NBitcoin;

var privateKey = new Key();
var publicKey = privateKey.PubKey;

Console.WriteLine($"Private Key:\n{privateKey.ToHex()}\n");
Console.WriteLine($"Public Key (compressed={publicKey.IsCompressed}):\n{publicKey.ToHex()}\n");

var publicKeySha256 = SHA256.HashData(publicKey.ToBytes());

Console.WriteLine($"SHA-256 of Public Key:\n{ToHex(publicKeySha256)}\n");

var publicKeySha256RipeMd160 = new RIPEMD160Managed().ComputeHash(publicKeySha256);

Console.WriteLine($"RIPEMD-160 of SHA-256 of Public Key:\n{ToHex(publicKeySha256RipeMd160)}\n");

var bits = new BitArray(publicKeySha256RipeMd160);
for (var i = 0; i < bits.Length; i++)
{
    if (i % 5 == 0)
    {
        Console.Write(" ");
    }

    Console.Write(bits[i] ? "1" : "0");
}
Console.WriteLine();

var ints = new int[32];
var intIndex = -1;
for (var i = 0; i < bits.Length; i++)
{
    if (i % 5 == 0)
    {
        intIndex++;
    }

    if (bits[i])
    {
        ints[intIndex] += (int)Math.Pow(2, 4 - i % 5);
    }
}

foreach (var intValue in ints)
{
    Console.Write($" {intValue:D2}   ");
}
Console.WriteLine();
foreach (var intValue in ints)
{
    Console.Write($" {intValue:x2}   ");
}
Console.WriteLine();

// What is wrong?
var hexStringWithWitnessVersion = "00";
foreach (var intValue in ints)
{
    hexStringWithWitnessVersion += $"{intValue:x2}";
}
Console.WriteLine($"HEX string with witness version:\n{hexStringWithWitnessVersion}\n");

var address = privateKey.GetAddress(ScriptPubKeyType.Segwit, Network.TestNet);

Console.WriteLine($"NBitcoin calculated Address:\n{address}\n");

static string ToHex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();
