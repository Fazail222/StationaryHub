using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;
using StationaryHub.Services;

namespace StationaryHub.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CartService _cartService;

    public CartController(AppDbContext context, UserManager<ApplicationUser> userManager, CartService cartService)
    {
        _context = context;
        _userManager = userManager;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index() => View(await _cartService.GetCartAsync(_userManager.GetUserId(User)!));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var userId = _userManager.GetUserId(User)!;
        var product = await _context.Products.FindAsync(productId);
        if (product is null || product.StockQuantity <= 0) return NotFound();
        quantity = Math.Clamp(quantity, 1, product.StockQuantity);
        var item = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        if (item is null)
        {
            _context.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = quantity });
        }
        else
        {
            item.Quantity = Math.Min(product.StockQuantity, item.Quantity + quantity);
        }
        await _context.SaveChangesAsync();
        TempData["Success"] = "Added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, int quantity)
    {
        var userId = _userManager.GetUserId(User)!;
        var item = await _context.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (item is null) return NotFound();
        item.Quantity = Math.Clamp(quantity, 1, item.Product!.StockQuantity);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (item is not null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var userId = _userManager.GetUserId(User)!;
        await _context.CartItems.Where(c => c.UserId == userId).ExecuteDeleteAsync();
        return RedirectToAction(nameof(Index));
    }
}
