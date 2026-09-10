using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Tests;

public class DatabaseTests
{
    [Fact]
    public async Task ApplicationDbContext_CanSaveAndReadCategory()
    {
        // Arrange
        await using var connection =
            new SqliteConnection(
                "Data Source=:memory:;Foreign Keys=True");

        await connection.OpenAsync();

        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var context =
            new ApplicationDbContext(options);

        await context.Database.EnsureCreatedAsync();

        var category = new Category
        {
            Name = "Utilities",
            Type = "Expense"
        };

        // Act
        context.Categories.Add(category);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var savedCategory =
            await context.Categories.SingleAsync();

        // Assert
        Assert.Equal("Utilities", savedCategory.Name);
        Assert.Equal("Expense", savedCategory.Type);
    }

    [Fact]
    public async Task ApplicationDbContext_PreventsDeletingCategoryUsedByTransaction()
    {
        // Arrange
        await using var connection =
            new SqliteConnection(
                "Data Source=:memory:;Foreign Keys=True");

        await connection.OpenAsync();

        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var context =
            new ApplicationDbContext(options);

        await context.Database.EnsureCreatedAsync();

        var category = new Category
        {
            Name = "Utilities",
            Type = "Expense"
        };

        context.Categories.Add(category);

        await context.SaveChangesAsync();

        var transaction = new Transaction
        {
            Description = "Electric Bill",
            Amount = 125.50m,
            Date = DateTime.Today,
            CategoryId = category.Id
        };

        context.Transactions.Add(transaction);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var categoryToDelete =
            await context.Categories.SingleAsync(
                savedCategory =>
                    savedCategory.Id == category.Id);

        // Act
        context.Categories.Remove(categoryToDelete);

        var exception =
            await Assert.ThrowsAsync<DbUpdateException>(
                () => context.SaveChangesAsync());

        // Assert
        Assert.NotNull(exception);
    }
}