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

    public BitcoinNodeClient(IOptions<BitcoinNodeClientOptions> options)
    {
        _rpcUrl = options.Value.RpcUrl;
        _rpcTimeout = options.Value.RpcTimeout;
    }

    // ... client methods
}