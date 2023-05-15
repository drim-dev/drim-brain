namespace Domain;

public class Deposit
{
    public int UserId { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public bool IsConfirmed { get; set; }
}