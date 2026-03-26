using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<VatRate> VatRates => Set<VatRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VatRate>().HasData(
            new VatRate { Id = 1, Name = "Standard Rate", Rate = 20m },
            new VatRate { Id = 2, Name = "Reduced Rate",  Rate = 5m  },
            new VatRate { Id = 3, Name = "Zero Rate",     Rate = 0m  }
        );
    }
}
