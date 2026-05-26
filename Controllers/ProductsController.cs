using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.ViewModels;

namespace StationaryHub.Controllers;

public class ProductsController : Controller
{
    private const int PageSize = 9;
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId, string? category, string? sort, int page = 1)
    {
        if (!string.IsNullOrWhiteSpace(category))
        {
            categoryId = await _context.Categories.Where(c => c.Name == category).Select(c => (int?)c.Id).FirstOrDefaultAsync();
        }

        var query = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId);

        query = sort switch
        {
            "price_desc" => query.OrderByDescending(p => p.Price),
            "newest" => query.OrderByDescending(p => p.CreatedDate),
            _ => query.OrderBy(p => p.Price)
        };

        var total = await query.CountAsync();
        page = Math.Max(1, page);
        var products = await query.Skip((page - 1) * PageSize).Take(PageSize)
            .Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category!.Name,
                CreatedDate = p.CreatedDate
            }).ToListAsync();

        return View(new ProductListViewModel
        {
            Products = products,
            Search = search,
            CategoryId = categoryId,
            Sort = sort,
            Page = page,
            TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize)),
            Categories = await _context.Categories.AsNoTracking()
                .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == categoryId))
                .ToListAsync()
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : View(product);
    }
}
