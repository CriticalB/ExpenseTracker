using System.Security.Claims;
using ExpenseTracker.DTOs;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var expenses = await _expenseService.GetAllAsync(userId);
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var expense = await _expenseService.GetByIdAsync(id, userId);

        if (expense == null) return NotFound();

        return Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto createExpense)
    {
        var userId = GetUserId();
        var expense = await _expenseService.CreateAsync(createExpense, userId);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto updatedExpense)
    {
        var userId = GetUserId();
        var expense = await _expenseService.UpdateAsync(id, updatedExpense, userId);

        if (expense == null) return NotFound();

        return Ok(expense);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var userId = GetUserId();
        var summary = await _expenseService.GetSummaryAsync(userId, from, to);
        return Ok(summary);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var deleted = await _expenseService.DeleteAsync(id, userId);

        if (!deleted) return NotFound();

        return NoContent();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);
}
