using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;

namespace StationaryHub.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var orders = await _context.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        return order is null ? NotFound() : View(order);
    }
}
