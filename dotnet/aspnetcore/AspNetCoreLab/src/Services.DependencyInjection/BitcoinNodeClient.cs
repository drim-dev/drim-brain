namespace Services.DependencyInjection;

public interface IBitcoinNodeClient
{
}

public class BitcoinNodeClient : IBitcoinNodeClient
{
    private readonly string _rpcUrl = "http://localhost:8332";
    private readonly TimeSpan _rpcTimeout = TimeSpan.FromMinutes(1);

    // ... client methods
}