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
            .Where(e => e.UserId == userId)
            .Select(e => ToDto(e))
            .ToListAsync();
    }

    public async Task<ExpenseResponseDto?> GetByIdAsync(int id, int userId)
    {
        var expense = await _context.Expenses
            .SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        return expense == null ? null : ToDto(expense);
    }

    public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto createExpense, int userId)
    {
        var expense = new Expense
        {
            Title = createExpense.Title,
            Amount = createExpense.Amount,
            Category = createExpense.Category,
            Date = createExpense.Date,
            Notes = createExpense.Notes,
            UserId = userId
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return ToDto(expense);
    }

    public async Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto updatedExpense, int userId)
    {
        var expense = await _context.Expenses
            .SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense == null) return null;

        expense.Title = updatedExpense.Title;
        expense.Amount = updatedExpense.Amount;
        expense.Category = updatedExpense.Category;
        expense.Date = updatedExpense.Date;
        expense.Notes = updatedExpense.Notes;

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

    private static ExpenseResponseDto ToDto(Expense expense) => new()
    {
        Id = expense.Id,
        Title = expense.Title,
        Amount = expense.Amount,
        Category = expense.Category,
        Date = expense.Date,
        Notes = expense.Notes
    };
}
