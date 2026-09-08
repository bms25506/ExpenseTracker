using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .OrderBy(category => category.Name)
            .ToListAsync();

        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int id)
    {
        var categoryToUpdate = await _context.Categories.FindAsync(id);

        if (categoryToUpdate is null)
        {
            return NotFound();
        }

        var updateSucceeded = await TryUpdateModelAsync(
            categoryToUpdate,
            "",
            category => category.Name,
            category => category.Type);

        if (updateSucceeded)
        {
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(categoryToUpdate);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var hasTransactions = await _context.Transactions
            .AnyAsync(transaction => transaction.CategoryId == id);

        if (hasTransactions)
        {
            TempData["CategoryDeleteError"] =
                "This category cannot be deleted because it is being used by one or more transactions.";

            return RedirectToAction(nameof(Index));
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}