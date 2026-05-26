using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StationaryHub.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
