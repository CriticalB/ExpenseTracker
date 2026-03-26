namespace ExpenseTracker.DTOs;

public class UpdateExpenseDto
{
    public string Title { get; set; } = string.Empty;
    public decimal NetAmount { get; set; }
    public decimal VatRate { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}
