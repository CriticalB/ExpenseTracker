namespace ExpenseTracker.Models;

public class Expense
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int VatRateId { get; set; }
    public VatRate VatRate { get; set; } = null!;
}
