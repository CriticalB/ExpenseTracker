namespace ExpenseTracker.DTOs;

public class ExpenseSummaryDto
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalNet { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalGross { get; set; }
    public int Count { get; set; }
}

public class ExpenseSummaryResponseDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalGross { get; set; }
    public List<ExpenseSummaryDto> Categories { get; set; } = new();
}
