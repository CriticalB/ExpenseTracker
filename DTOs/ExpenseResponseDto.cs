namespace ExpenseTracker.DTOs;

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal VatRate { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}
