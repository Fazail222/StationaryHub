using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;
using StationaryHub.ViewModels;

namespace StationaryHub.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var orders = _context.Orders.AsNoTracking();
        var revenueByStatus = await orders.GroupBy(o => o.Status.ToString()).ToDictionaryAsync(g => g.Key, g => g.Sum(o => o.TotalAmount));
        return View(new AdminDashboardViewModel
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalOrders = await orders.CountAsync(),
            TotalProducts = await _context.Products.CountAsync(),
            TotalRevenue = await orders.Where(o => o.Status != OrderStatus.Cancelled).SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
            RecentOrders = await _context.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).Take(8).ToListAsync(),
            RevenueByStatus = revenueByStatus
        });
    }
}
