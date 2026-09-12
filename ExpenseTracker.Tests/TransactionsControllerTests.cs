using ExpenseTracker.Controllers;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Tests;

public class TransactionsControllerTests
{
    [Fact]
    public async Task Index_WithSearchTerm_ReturnsMatchingTransactions()
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

        context.Transactions.AddRange(
            new Transaction
            {
                Description = "Electric Bill",
                Amount = 125.50m,
                Date = new DateTime(2026, 9, 1),
                CategoryId = category.Id
            },
            new Transaction
            {
                Description = "Grocery Store",
                Amount = 75.25m,
                Date = new DateTime(2026, 9, 2),
                CategoryId = category.Id
            });

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var controller =
            new TransactionsController(context);

        // Act
        var result = await controller.Index(
            searchTerm: "Electric",
            categoryId: null,
            startDate: null,
            endDate: null,
            sortBy: "date",
            sortDirection: "desc",
            page: 1);

        // Assert
        var viewResult =
            Assert.IsType<ViewResult>(result);

        var viewModel =
            Assert.IsType<TransactionIndexViewModel>(
                viewResult.Model);

        var transaction =
            Assert.Single(viewModel.Transactions);

        Assert.Equal(
            "Electric Bill",
            transaction.Description);

        Assert.Equal(
            125.50m,
            viewModel.TotalExpenses);

        Assert.Equal(
            0m,
            viewModel.TotalIncome);

        Assert.Equal(
            -125.50m,
            viewModel.Balance);
    }

    [Fact]
    public async Task Index_WithCategoryId_ReturnsOnlyMatchingCategory()
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

        var utilitiesCategory = new Category
        {
            Name = "Utilities",
            Type = "Expense"
        };

        var groceriesCategory = new Category
        {
            Name = "Groceries",
            Type = "Expense"
        };

        context.Categories.AddRange(
            utilitiesCategory,
            groceriesCategory);

        await context.SaveChangesAsync();

        context.Transactions.AddRange(
            new Transaction
            {
                Description = "Electric Bill",
                Amount = 125.50m,
                Date = new DateTime(2026, 9, 1),
                CategoryId = utilitiesCategory.Id
            },
            new Transaction
            {
                Description = "Grocery Store",
                Amount = 75.25m,
                Date = new DateTime(2026, 9, 2),
                CategoryId = groceriesCategory.Id
            });

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var controller =
            new TransactionsController(context);

        // Act
        var result = await controller.Index(
            searchTerm: null,
            categoryId: groceriesCategory.Id,
            startDate: null,
            endDate: null,
            sortBy: "date",
            sortDirection: "desc",
            page: 1);

        // Assert
        var viewResult =
            Assert.IsType<ViewResult>(result);

        var viewModel =
            Assert.IsType<TransactionIndexViewModel>(
                viewResult.Model);

        var transaction =
            Assert.Single(viewModel.Transactions);

        Assert.Equal(
            "Grocery Store",
            transaction.Description);

        Assert.Equal(
            groceriesCategory.Id,
            transaction.CategoryId);

        Assert.Equal(
            75.25m,
            viewModel.TotalExpenses);

        Assert.Equal(
            0m,
            viewModel.TotalIncome);

        Assert.Equal(
            -75.25m,
            viewModel.Balance);
    }

    [Fact]
    public async Task Index_WithAmountAscending_ReturnsTransactionsInAmountOrder()
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

        context.Transactions.AddRange(
            new Transaction
            {
                Description = "Electric Bill",
                Amount = 125.50m,
                Date = new DateTime(2026, 9, 1),
                CategoryId = category.Id
            },
            new Transaction
            {
                Description = "Water Bill",
                Amount = 25.00m,
                Date = new DateTime(2026, 9, 2),
                CategoryId = category.Id
            },
            new Transaction
            {
                Description = "Internet Bill",
                Amount = 75.00m,
                Date = new DateTime(2026, 9, 3),
                CategoryId = category.Id
            });

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var controller =
            new TransactionsController(context);

        // Act
        var result = await controller.Index(
            searchTerm: null,
            categoryId: null,
            startDate: null,
            endDate: null,
            sortBy: "amount",
            sortDirection: "asc",
            page: 1);

        // Assert
        var viewResult =
            Assert.IsType<ViewResult>(result);

        var viewModel =
            Assert.IsType<TransactionIndexViewModel>(
                viewResult.Model);

        Assert.Equal(
            "amount",
            viewModel.SortBy);

        Assert.Equal(
            "asc",
            viewModel.SortDirection);

        Assert.Collection(
            viewModel.Transactions,
            firstTransaction =>
                Assert.Equal(
                    25.00m,
                    firstTransaction.Amount),
            secondTransaction =>
                Assert.Equal(
                    75.00m,
                    secondTransaction.Amount),
            thirdTransaction =>
                Assert.Equal(
                    125.50m,
                    thirdTransaction.Amount));
    }
}