using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseResponseDto>> GetAllAsync(int userId)
    {
        return await _context.Expenses
            .Include(e => e.VatRate)
            .Where(e => e.UserId == userId)
            .Select(e => ToDto(e))
            .ToListAsync();
    }

    public async Task<ExpenseResponseDto?> GetByIdAsync(int id, int userId)
    {
        var expense = await _context.Expenses
            .Include(e => e.VatRate)
            .SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        return expense == null ? null : ToDto(expense);
    }

    public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto createExpense, int userId)
    {
        var vatRate = await GetVatRateByValueAsync(createExpense.VatRate);

        var expense = new Expense
        {
            Title = createExpense.Title,
            NetAmount = createExpense.NetAmount,
            VatAmount = createExpense.NetAmount * (vatRate.Rate / 100),
            Category = createExpense.Category,
            Date = createExpense.Date,
            Notes = createExpense.Notes,
            UserId = userId,
            VatRateId = vatRate.Id
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        expense.VatRate = vatRate;
        return ToDto(expense);
    }

    public async Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto updatedExpense, int userId)
    {
        var expense = await _context.Expenses
            .Include(e => e.VatRate)
            .SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense == null) return null;

        var vatRate = await GetVatRateByValueAsync(updatedExpense.VatRate);

        expense.Title = updatedExpense.Title;
        expense.NetAmount = updatedExpense.NetAmount;
        expense.VatAmount = updatedExpense.NetAmount * (vatRate.Rate / 100);
        expense.Category = updatedExpense.Category;
        expense.Date = updatedExpense.Date;
        expense.Notes = updatedExpense.Notes;
        expense.VatRateId = vatRate.Id;
        expense.VatRate = vatRate;

        await _context.SaveChangesAsync();

        return ToDto(expense);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var expense = await _context.Expenses
            .SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense == null) return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ExpenseSummaryResponseDto> GetSummaryAsync(int userId, DateTime? from, DateTime? to)
    {
        var query = _context.Expenses.Where(e => e.UserId == userId);

        if (from.HasValue)
            query = query.Where(e => e.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.Date <= to.Value);

        var categories = await query
            .GroupBy(e => e.Category)
            .Select(g => new ExpenseSummaryDto
            {
                Category = g.Key,
                TotalNet = g.Sum(e => e.NetAmount),
                TotalVat = g.Sum(e => e.VatAmount),
                TotalGross = g.Sum(e => e.NetAmount + e.VatAmount),
                Count = g.Count()
            })
            .ToListAsync();

        return new ExpenseSummaryResponseDto
        {
            From = from,
            To = to,
            TotalNet = categories.Sum(c => c.TotalNet),
            TotalVat = categories.Sum(c => c.TotalVat),
            TotalGross = categories.Sum(c => c.TotalGross),
            Categories = categories
        };
    }

    private async Task<VatRate> GetVatRateByValueAsync(decimal rate)
    {
        var vatRate = await _context.VatRates.SingleOrDefaultAsync(v => v.Rate == rate);

        if (vatRate == null)
            throw new InvalidOperationException($"No VAT rate configured for {rate}%.");

        return vatRate;
    }

    private static ExpenseResponseDto ToDto(Expense expense) => new()
    {
        Id = expense.Id,
        Title = expense.Title,
        NetAmount = expense.NetAmount,
        VatAmount = expense.VatAmount,
        GrossAmount = expense.NetAmount + expense.VatAmount,
        VatRate = expense.VatRate.Rate,
        Category = expense.Category,
        Date = expense.Date,
        Notes = expense.Notes
    };
}
