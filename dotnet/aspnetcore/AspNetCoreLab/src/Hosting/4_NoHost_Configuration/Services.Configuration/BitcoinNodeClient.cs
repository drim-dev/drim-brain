using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Configuration.Options;

namespace Services.Configuration;

public interface IBitcoinNodeClient
{
}

public class BitcoinNodeClient : IBitcoinNodeClient
{
    private readonly string _rpcUrl;
    private readonly TimeSpan _rpcTimeout;

    public BitcoinNodeClient(IConfiguration configuration)
    {
        _rpcUrl = configuration.GetSection("BitcoinNodeClient:RpcUrl").Get<string>()!;
        _rpcTimeout = configuration.GetSection("BitcoinNodeClient:RpcTimeout").Get<TimeSpan>();
    }

    // ... client methods
}