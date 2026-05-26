using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StationaryHub.ViewModels;

public class ProductListViewModel
{
    public IReadOnlyList<ProductCardViewModel> Products { get; set; } = [];
    public IReadOnlyList<SelectListItem> Categories { get; set; } = [];
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}

public class ProductCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class ProductEditViewModel
{
    public int Id { get; set; }

    [Required, StringLength(140)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(1200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 999999)]
    public decimal Price { get; set; }

    [Range(0, 100000)]
    public int StockQuantity { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public IFormFile? ImageFile { get; set; }
    public IReadOnlyList<SelectListItem> Categories { get; set; } = [];
}
