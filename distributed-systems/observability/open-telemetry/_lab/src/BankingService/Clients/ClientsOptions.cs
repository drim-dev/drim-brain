namespace BankingService.Clients;

public class ClientsOptions
{
    public const string SectionName = "Clients";

    public required string BlockchainService { get; set; }
}
