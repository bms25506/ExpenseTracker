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

    public string SortBy {get;set;} = "date";

    public string SortDirection {get;set;} = "desc";

    public decimal TotalIncome {get;set;}

    public decimal TotalExpenses {get;set;}

    public decimal Balance {get;set;}

    public int CurrentPage {get;set;} = 1;

    public int PageSize {get;set;} = 10;

    public int TotalItems {get;set;}

    public int TotalPages {get;set;}

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    public List<SelectListItem> Categories {get;set;} = new();

    public List<Transaction> Transactions {get;set;} = new();
}