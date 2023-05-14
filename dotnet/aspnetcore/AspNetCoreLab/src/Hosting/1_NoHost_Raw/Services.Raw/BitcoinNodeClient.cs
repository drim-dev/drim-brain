namespace Services.Raw;

public class BitcoinNodeClient
{
    private readonly string _rpcUrl = "http://localhost:8332";
    private readonly TimeSpan _rpcTimeout = TimeSpan.FromMinutes(1);

    // ... client methods
}