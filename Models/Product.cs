using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationaryHub.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(140)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(1200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 999999), Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Range(0, 100000)]
    public int StockQuantity { get; set; }

    [Required, StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
