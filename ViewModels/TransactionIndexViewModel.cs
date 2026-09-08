using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.ViewModels;

public class TransactionIndexViewModel
{
    public string SearchTerm {get;set;} = string.Empty;

    public int? CategoryId {get;set;}

    [DataType(DataType.Date)]
    public DateTime? StartDate {get;set;}

    [DataType(DataType.Date)]
    public DateTime? EndDate {get;set;}

    public List<SelectListItem> Categories {get;set;} = new();

    public List<Transaction> Transactions {get;set;} = new();
}