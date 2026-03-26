using ExpenseTracker.DTOs;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    Task<List<ExpenseResponseDto>> GetAllAsync(int userId);
    Task<ExpenseResponseDto?> GetByIdAsync(int id, int userId);
    Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto createExpense, int userId);
    Task<ExpenseResponseDto?> UpdateAsync(int id, UpdateExpenseDto updatedExpense, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
