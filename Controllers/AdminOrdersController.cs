using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;

namespace StationaryHub.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminOrdersController : Controller
{
    private readonly AppDbContext _context;

    public AdminOrdersController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders.Include(o => o.User).Include(o => o.OrderItems).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);
        return order is null ? NotFound() : View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();
        order.Status = status;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return NotFound();

        if (order.Status != OrderStatus.Delivered)
        {
            TempData["Success"] = "Only delivered orders can be deleted.";
            return RedirectToAction(nameof(Index));
        }

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Order #{id} was deleted.";
        return RedirectToAction(nameof(Index));
    }
}
