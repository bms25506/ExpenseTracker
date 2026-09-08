using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(
        100,
        ErrorMessage = "Description cannot be longer than 100 characters.")]
    public string Description { get; set; } = string.Empty;

    [Range(
        0.01,
        1000000,
        ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}