using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Tests;

public class ExpenseServiceTests
{
    // Each test gets its own named database so tests don't share state
    private AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new AppDbContext(options);

        context.VatRates.AddRange(
            new VatRate { Id = 1, Name = "Standard Rate", Rate = 20m },
            new VatRate { Id = 2, Name = "Reduced Rate",  Rate = 5m  },
            new VatRate { Id = 3, Name = "Zero Rate",     Rate = 0m  }
        );
        context.SaveChanges();

        return context;
    }

    // GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ReturnsExpense_WhenItBelongsToUser()
    {
        var context = CreateContext(nameof(GetByIdAsync_ReturnsExpense_WhenItBelongsToUser));
        context.Expenses.Add(new Expense
        {
            Id = 1, Title = "Office Chair", NetAmount = 100m, VatAmount = 20m,
            Category = "Equipment", Date = DateTime.UtcNow, UserId = 1, VatRateId = 1
        });
        context.SaveChanges();

        var service = new ExpenseService(context);
        var result = await service.GetByIdAsync(1, userId: 1);

        Assert.NotNull(result);
        Assert.Equal("Office Chair", result.Title);
        Assert.Equal(100m, result.NetAmount);
        Assert.Equal(20m, result.VatRate);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenExpenseDoesNotExist()
    {
        var context = CreateContext(nameof(GetByIdAsync_ReturnsNull_WhenExpenseDoesNotExist));
        var service = new ExpenseService(context);

        var result = await service.GetByIdAsync(99, userId: 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenExpenseBelongsToAnotherUser()
    {
        var context = CreateContext(nameof(GetByIdAsync_ReturnsNull_WhenExpenseBelongsToAnotherUser));
        context.Expenses.Add(new Expense
        {
            Id = 1, Title = "Laptop", NetAmount = 800m, VatAmount = 160m,
            Category = "Equipment", Date = DateTime.UtcNow, UserId = 2, VatRateId = 1
        });
        context.SaveChanges();

        var service = new ExpenseService(context);
        var result = await service.GetByIdAsync(1, userId: 1);

        Assert.Null(result);
    }

    // CreateAsync

    [Fact]
    public async Task CreateAsync_CreatesExpense_WithCorrectVatAmount()
    {
        var context = CreateContext(nameof(CreateAsync_CreatesExpense_WithCorrectVatAmount));
        var service = new ExpenseService(context);

        var dto = new CreateExpenseDto
        {
            Title = "Monitor", NetAmount = 200m, VatRate = 20m,
            Category = "Equipment", Date = DateTime.UtcNow
        };

        var result = await service.CreateAsync(dto, userId: 1);

        Assert.Equal("Monitor", result.Title);
        Assert.Equal(200m, result.NetAmount);
        Assert.Equal(40m, result.VatAmount);    // 200 * 20% = 40
        Assert.Equal(240m, result.GrossAmount);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenVatRateIsInvalid()
    {
        var context = CreateContext(nameof(CreateAsync_Throws_WhenVatRateIsInvalid));
        var service = new ExpenseService(context);

        var dto = new CreateExpenseDto
        {
            Title = "Unknown", NetAmount = 100m, VatRate = 99m,
            Category = "Other", Date = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(dto, userId: 1));
    }

    // DeleteAsync

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenExpenseDeleted()
    {
        var context = CreateContext(nameof(DeleteAsync_ReturnsTrue_WhenExpenseDeleted));
        context.Expenses.Add(new Expense
        {
            Id = 1, Title = "Desk", NetAmount = 300m, VatAmount = 60m,
            Category = "Furniture", Date = DateTime.UtcNow, UserId = 1, VatRateId = 1
        });
        context.SaveChanges();

        var service = new ExpenseService(context);
        var result = await service.DeleteAsync(1, userId: 1);

        Assert.True(result);
        Assert.Empty(context.Expenses);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenExpenseDoesNotExist()
    {
        var context = CreateContext(nameof(DeleteAsync_ReturnsFalse_WhenExpenseDoesNotExist));
        var service = new ExpenseService(context);

        var result = await service.DeleteAsync(99, userId: 1);

        Assert.False(result);
    }
}
