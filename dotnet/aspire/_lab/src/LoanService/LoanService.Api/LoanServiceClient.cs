using System.Net.Http.Json;

namespace LoanService.Api;

public class LoanServiceClient(HttpClient _httpClient)
{
    public async Task<LoanOfferingDto[]> GetOfferings(CancellationToken cancellationToken) =>
        await _httpClient.GetFromJsonAsync<LoanOfferingDto[]>("/offerings", cancellationToken) ?? [];
}

public record LoanOfferingDto(string Name, int Months, decimal InterestRate, decimal MaxAmount);
