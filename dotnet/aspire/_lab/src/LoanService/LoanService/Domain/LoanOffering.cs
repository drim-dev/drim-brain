namespace LoanService.Domain;

public class LoanOffering(string name, int months, decimal interestRate, decimal maxAmount)
{
    public string Name { get; } = name;
    public int Months { get; } = months;
    public decimal InterestRate { get; } = interestRate;
    public decimal MaxAmount { get; } = maxAmount;
}
