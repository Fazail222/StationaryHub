using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.ViewModels;

namespace StationaryHub.Services;

public class CartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CartViewModel> GetCartAsync(string userId)
    {
        var items = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Product!.Name)
            .Select(c => new CartItemViewModel
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product!.Name,
                ImageUrl = c.Product.ImageUrl,
                UnitPrice = c.Product.Price,
                Quantity = c.Quantity,
                StockQuantity = c.Product.StockQuantity
            })
            .ToListAsync();

        return new CartViewModel { Items = items };
    }
}
