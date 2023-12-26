using System.Net.Http.Json;

namespace BlockchainService.Api;

public class BlockchainServiceClient(HttpClient _httpClient)
{
    public async Task Withdraw(WithdrawalDto withdrawal, CancellationToken cancellationToken) =>
        await _httpClient.PostAsJsonAsync("/withdrawals", withdrawal, cancellationToken);
}

public record WithdrawalDto(int AccountId, decimal Amount, string CryptoAddress);
