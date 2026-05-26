using System.ComponentModel.DataAnnotations;

namespace StationaryHub.ViewModels;

public class CheckoutViewModel
{
    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;
}
