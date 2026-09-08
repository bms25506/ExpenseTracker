using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.ViewModels;

public class TransactionCreateViewModel
{
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
    public DateTime Date { get; set; } = DateTime.Today;

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
}