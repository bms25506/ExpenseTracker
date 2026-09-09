using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TransactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? searchTerm,
        int? categoryId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _context.Transactions
            .AsQueryable();

        var invalidDateRange =
            startDate.HasValue &&
            endDate.HasValue &&
            startDate.Value.Date > endDate.Value.Date;

        if (invalidDateRange)
        {
            ModelState.AddModelError(
                nameof(TransactionIndexViewModel.EndDate),
                "End date must be on or after the start date.");
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var trimmedSearchTerm = searchTerm.Trim();

            query = query.Where(transaction =>
                transaction.Description.Contains(trimmedSearchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(transaction =>
                transaction.CategoryId == categoryId.Value);
        }

        if (!invalidDateRange && startDate.HasValue)
        {
            var start = startDate.Value.Date;

            query = query.Where(transaction =>
                transaction.Date >= start);
        }

        if (!invalidDateRange && endDate.HasValue)
        {
            var endExclusive = endDate.Value.Date.AddDays(1);

            query = query.Where(transaction =>
                transaction.Date < endExclusive);
        }

        var totalIncome = await query
            .Where(transaction =>
                transaction.Category != null &&
                transaction.Category.Type == "Income")
            .SumAsync(transaction => transaction.Amount);

        var totalExpenses = await query
            .Where(transaction =>
                transaction.Category != null &&
                transaction.Category.Type == "Expense")
            .SumAsync(transaction => transaction.Amount);

        var transactions = await query
            .Include(transaction => transaction.Category)
            .OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.Id)
            .ToListAsync();

        var viewModel = new TransactionIndexViewModel
        {
            SearchTerm = searchTerm?.Trim() ?? string.Empty,
            CategoryId = categoryId,
            StartDate = startDate,
            EndDate = endDate,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses,
            Categories = await GetCategorySelectListAsync(),
            Transactions = transactions
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new TransactionCreateViewModel
        {
            Categories = await GetCategorySelectListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        TransactionCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(category => category.Id == viewModel.CategoryId);

            if (!categoryExists)
            {
                ModelState.AddModelError(
                    nameof(viewModel.CategoryId),
                    "The selected category does not exist.");
            }
        }

        if (!ModelState.IsValid)
        {
            viewModel.Categories = await GetCategorySelectListAsync();

            return View(viewModel);
        }

        var transaction = new Transaction
        {
            Description = viewModel.Description.Trim(),
            Amount = viewModel.Amount,
            Date = viewModel.Date,
            CategoryId = viewModel.CategoryId
        };

        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var transaction = await _context.Transactions
            .FindAsync(id);

        if (transaction is null)
        {
            return NotFound();
        }

        var viewModel = new TransactionCreateViewModel
        {
            Description = transaction.Description,
            Amount = transaction.Amount,
            Date = transaction.Date,
            CategoryId = transaction.CategoryId,
            Categories = await GetCategorySelectListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(
        int id,
        TransactionCreateViewModel viewModel)
    {
        var transactionToUpdate = await _context.Transactions
            .FindAsync(id);

        if (transactionToUpdate is null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(category => category.Id == viewModel.CategoryId);

            if (!categoryExists)
            {
                ModelState.AddModelError(
                    nameof(viewModel.CategoryId),
                    "The selected category does not exist.");
            }
        }

        if (!ModelState.IsValid)
        {
            viewModel.Categories = await GetCategorySelectListAsync();

            return View("Edit", viewModel);
        }

        transactionToUpdate.Description = viewModel.Description.Trim();
        transactionToUpdate.Amount = viewModel.Amount;
        transactionToUpdate.Date = viewModel.Date;
        transactionToUpdate.CategoryId = viewModel.CategoryId;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = await _context.Transactions
            .Include(transaction => transaction.Category)
            .FirstOrDefaultAsync(transaction => transaction.Id == id);

        if (transaction is null)
        {
            return NotFound();
        }

        return View(transaction);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transaction = await _context.Transactions
            .FindAsync(id);

        if (transaction is null)
        {
            return RedirectToAction(nameof(Index));
        }

        _context.Transactions.Remove(transaction);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetCategorySelectListAsync()
    {
        return await _context.Categories
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = $"{category.Name} ({category.Type})"
            })
            .ToListAsync();
    }
}