using StationaryHub.Models;

namespace StationaryHub.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public decimal TotalRevenue { get; set; }
    public IReadOnlyList<Order> RecentOrders { get; set; } = [];
    public IReadOnlyDictionary<string, decimal> RevenueByStatus { get; set; } = new Dictionary<string, decimal>();
}
