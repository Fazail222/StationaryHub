using StationaryHub.Models;

namespace StationaryHub.ViewModels;

public class HomeViewModel
{
    public IReadOnlyList<Product> FeaturedProducts { get; set; } = [];
    public IReadOnlyList<Product> NewArrivals { get; set; } = [];
    public IReadOnlyList<Category> Categories { get; set; } = [];
}
