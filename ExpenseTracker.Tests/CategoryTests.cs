using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Models;

namespace ExpenseTracker.Tests;

public class CategoryTests
{
    [Fact]
    public void Category_WithEmptyName_IsInvalid()
    {
        // Arrange
        var category = new Category
        {
            Name = string.Empty,
            Type = "Expense"
        };

        var validationContext =
            new ValidationContext(category);

        var validationResults =
            new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            category,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);

        Assert.Contains(
            validationResults,
            result =>
                result.MemberNames.Contains(nameof(Category.Name)));
    }

    [Fact]
    public void Category_WithValidValues_IsValid()
    {
        // Arrange
        var category = new Category
        {
            Name = "Utilities",
            Type = "Expense"
        };

        var validationContext =
            new ValidationContext(category);

        var validationResults =
            new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            category,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }
}