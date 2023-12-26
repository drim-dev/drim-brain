using System.Net.Http.Json;

namespace AccountService.Api;

public class AccountServiceClient(HttpClient _httpClient)
{
    public async Task Withdraw(WithdrawalDto withdrawal, CancellationToken cancellationToken) =>
        await _httpClient.PostAsJsonAsync("/withdrawals", withdrawal, cancellationToken);
}

public record WithdrawalDto(int AccountId, decimal Amount, string CryptoAddress);
