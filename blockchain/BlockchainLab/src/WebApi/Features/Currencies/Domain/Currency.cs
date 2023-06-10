namespace WebApi.Features.Currencies.Domain;

public class Currency
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}
