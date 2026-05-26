using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Data;
using StationaryHub.Models;
using StationaryHub.ViewModels;
using System.Diagnostics;

namespace StationaryHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                Categories = await _context.Categories.AsNoTracking().ToListAsync(),
                FeaturedProducts = await _context.Products.Include(p => p.Category).OrderBy(p => Guid.NewGuid()).Take(6).ToListAsync(),
                NewArrivals = await _context.Products.Include(p => p.Category).OrderByDescending(p => p.CreatedDate).Take(6).ToListAsync()
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
