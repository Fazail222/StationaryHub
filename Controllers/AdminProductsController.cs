using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;
using StationaryHub.Services;
using StationaryHub.ViewModels;

namespace StationaryHub.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminProductsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IImageStorageService _imageStorage;

    public AdminProductsController(AppDbContext context, IImageStorageService imageStorage)
    {
        _context = context;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search) ||
                p.Category!.Name.Contains(search));
        }

        ViewData["Search"] = search;
        return View(await query.OrderBy(p => p.Name).ToListAsync());
    }

    public async Task<IActionResult> Create() => View(await PrepareAsync(new ProductEditViewModel()));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductEditViewModel model)
    {
        if (!ModelState.IsValid) return View(await PrepareAsync(model));
        try
        {
            var image = await _imageStorage.SaveProductImageAsync(model.ImageFile);
            _context.Products.Add(new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                ImageUrl = image ?? model.ImageUrl ?? "https://placehold.co/900x700/0f172a/a3e635?text=StationeryHub",
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
            return View(await PrepareAsync(model));
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        return View(await PrepareAsync(new ProductEditViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(await PrepareAsync(model));
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        try
        {
            var image = await _imageStorage.SaveProductImageAsync(model.ImageFile);
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.ImageUrl = image ?? model.ImageUrl ?? product.ImageUrl;
            product.CategoryId = model.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
            return View(await PrepareAsync(model));
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<ProductEditViewModel> PrepareAsync(ProductEditViewModel model)
    {
        model.Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == model.CategoryId)).ToListAsync();
        return model;
    }
}
