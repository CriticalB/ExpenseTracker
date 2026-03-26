namespace ExpenseTracker.DTOs;

public class CreateExpenseDto
{
    public string Title { get; set; } = string.Empty;
    public decimal NetAmount { get; set; }
    public decimal VatRate { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
