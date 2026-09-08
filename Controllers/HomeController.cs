using System.Diagnostics;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalIncome = await _context.Transactions
            .Where(transaction =>
                transaction.Category != null &&
                transaction.Category.Type == "Income")
            .SumAsync(transaction => transaction.Amount);

        var totalExpenses = await _context.Transactions
            .Where(transaction =>
                transaction.Category != null &&
                transaction.Category.Type == "Expense")
            .SumAsync(transaction => transaction.Amount);

        var recentTransactions = await _context.Transactions
            .Include(transaction => transaction.Category)
            .OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.Id)
            .Take(5)
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses,
            RecentTransactions = recentTransactions
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
    }
}