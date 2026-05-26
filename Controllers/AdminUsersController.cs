using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;

namespace StationaryHub.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminUsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public AdminUsersController(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index() => View(await _userManager.Users.OrderBy(u => u.Email).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Success"] = "You cannot delete the account you are currently using.";
            return RedirectToAction(nameof(Index));
        }

        if (await _userManager.IsInRoleAsync(user, Roles.Admin))
        {
            TempData["Success"] = "Admin accounts cannot be deleted from this screen.";
            return RedirectToAction(nameof(Index));
        }

        var orders = await _context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == id).ToListAsync();
        _context.OrderItems.RemoveRange(orders.SelectMany(o => o.OrderItems));
        _context.Orders.RemoveRange(orders);

        var cartItems = await _context.CartItems.Where(c => c.UserId == id).ToListAsync();
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        var result = await _userManager.DeleteAsync(user);
        TempData["Success"] = result.Succeeded
            ? $"User {user.Email} was deleted."
            : string.Join(" ", result.Errors.Select(e => e.Description));

        return RedirectToAction(nameof(Index));
    }
}
