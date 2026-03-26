namespace ExpenseTracker.Models;

public class VatRate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
}
