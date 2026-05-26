using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;

namespace StationaryHub.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminCategoriesController : Controller
{
    private readonly AppDbContext _context;

    public AdminCategoriesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index() => View(await _context.Categories.Include(c => c.Products).OrderBy(c => c.Name).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
