namespace GenericHost.Kestrel.Endpoints.Dtos;

public class DepositDto
{
    public int UserId { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public bool IsConfirmed { get; set; }
}
