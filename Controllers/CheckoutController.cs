using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;
using StationaryHub.ViewModels;

namespace StationaryHub.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CheckoutController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();
        return View(new CheckoutViewModel { FullName = user.FullName, Phone = user.PhoneNumber ?? string.Empty });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        var userId = _userManager.GetUserId(User)!;
        var cartItems = await _context.CartItems.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();
        if (cartItems.Count == 0) ModelState.AddModelError(string.Empty, "Your cart is empty.");
        foreach (var item in cartItems.Where(i => i.Product!.StockQuantity < i.Quantity))
        {
            ModelState.AddModelError(string.Empty, $"{item.Product!.Name} does not have enough stock.");
        }
        if (!ModelState.IsValid) return View(model);

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = $"{model.FullName} | {model.Phone} | {model.Address}, {model.City} {model.PostalCode}",
            TotalAmount = cartItems.Sum(i => i.Product!.Price * i.Quantity),
            Status = OrderStatus.Pending
        };

        foreach (var item in cartItems)
        {
            order.OrderItems.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity, UnitPrice = item.Product!.Price });
            item.Product.StockQuantity -= item.Quantity;
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }
}
