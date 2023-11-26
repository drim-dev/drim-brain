namespace ApiGateway.Clients;

public class ClientsOptions
{
    public const string SectionName = "Clients";

    public required string BankingService { get; set; }
}
