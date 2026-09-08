using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Category
{
    public int Id {get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(
        50,
        ErrorMessage = "Category name cannot be longer than 50 characters.")]

    public string Name {get; set; } = string.Empty;

    [Required(ErrorMessage = "Category type is required.")]
    public string Type {get; set; } = string.Empty;

    public ICollection<Transaction> Transactions {get; set; } = new List<Transaction>();
}