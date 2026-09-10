using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Models;

namespace ExpenseTracker.Tests;

public class TransactionTests
{
    [Fact]
    public void Transaction_WithZeroAmount_IsInvalid()
    {
        // Arrange
        var transaction = new Transaction
        {
            Description = "Electric Bill",
            Amount = 0,
            Date = DateTime.Today,
            CategoryId = 1
        };

        var validationContext =
            new ValidationContext(transaction);

        var validationResults =
            new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            transaction,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);

        Assert.Contains(
            validationResults,
            result =>
                result.MemberNames.Contains(
                    nameof(Transaction.Amount)));
    }

    [Fact]
    public void Transaction_WithValidValues_IsValid()
    {
        // Arrange
        var transaction = new Transaction
        {
            Description = "Electric Bill",
            Amount = 125.50m,
            Date = DateTime.Today,
            CategoryId = 1
        };

        var validationContext =
            new ValidationContext(transaction);

        var validationResults =
            new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            transaction,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }
}