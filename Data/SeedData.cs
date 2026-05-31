using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StationaryHub.Models;

namespace StationaryHub.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<AppDbContext>();

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { Roles.Admin, Roles.Customer })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@stationeryhub.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "StationeryHub Admin",
                PhoneNumber = "555-0100"
            };
            await userManager.CreateAsync(admin, "Admin@123");
        }

        if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Stationery" },
                new Category { Name = "Sports" },
                new Category { Name = "Toys" });
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
            context.Products.AddRange(BuildProducts(categories));
            await context.SaveChangesAsync();
        }
    }

    private static IEnumerable<Product> BuildProducts(Dictionary<string, int> categories)
    {
        var now = DateTime.UtcNow;
        var products = new List<Product>();
        Add(products, categories["Stationery"], now, new[]
        {
            ("Executive Notebook Set", "Hardcover dotted notebooks with smooth 100 gsm paper.", 18.99m, 75),
            ("Precision Gel Pen Pack", "Twelve quick-dry gel pens for clean notes and sketches.", 11.49m, 120),
            ("Premium Desk Organizer", "Matte metal organizer with trays for letters, pens and clips.", 29.95m, 40),
            ("Academic Planner 2026", "Weekly and monthly planner with goal pages and project trackers.", 16.50m, 64),
            ("Artist Colored Pencils", "Soft-core 48 color pencil set for illustration and coloring.", 24.00m, 55),
            ("Sticky Notes Collection", "Assorted sticky notes, page tabs and memo pads.", 8.75m, 150),
            ("Geometry Essentials Kit", "Ruler, compass, protractor and triangle set in a zip case.", 9.99m, 90),
            ("Fountain Pen Starter", "Smooth fountain pen with converter and two ink cartridges.", 22.99m, 35),
            ("A4 Copy Paper Ream", "Bright white 500-sheet multipurpose paper ream.", 7.99m, 200),
            ("Minimal Pencil Case", "Durable water-resistant case with internal dividers.", 13.25m, 80)
        });
        Add(products, categories["Sports"], now, new[]
        {
            ("Street Basketball", "Grip-textured size 7 basketball for indoor and outdoor play.", 27.99m, 44),
            ("Training Soccer Ball", "Durable size 5 soccer ball with balanced flight.", 24.49m, 58),
            ("Adjustable Jump Rope", "Speed rope with ergonomic handles and adjustable cable.", 12.99m, 92),
            ("Yoga Mat Pro", "Non-slip 6 mm mat for studio, home and travel workouts.", 31.00m, 38),
            ("Tennis Ball Can", "Three high-bounce tennis balls for practice and matches.", 6.99m, 140),
            ("Resistance Band Set", "Five latex bands with progressive resistance levels.", 17.50m, 73),
            ("Sports Water Bottle", "Insulated 24 oz bottle with leak-proof cap.", 19.99m, 65),
            ("Badminton Racket Duo", "Two lightweight rackets with shuttlecocks and carry bag.", 34.95m, 27),
            ("Goalkeeper Gloves", "Padded gloves with strong wrist support and grip palms.", 28.00m, 31),
            ("Running Waist Pack", "Slim reflective belt with phone pocket and key clip.", 15.75m, 86)
        });
        Add(products, categories["Toys"], now, new[]
        {
            ("Creative Building Blocks", "300-piece colorful block set for imaginative construction.", 32.99m, 50),
            ("Wooden Puzzle Board", "Shape and pattern puzzle that builds logic skills.", 19.49m, 66),
            ("Remote Control Racer", "Rechargeable RC car with responsive steering.", 39.99m, 22),
            ("Plush Dino Friend", "Soft huggable dinosaur plush with embroidered details.", 14.95m, 77),
            ("Science Experiment Kit", "Hands-on chemistry and physics activities for curious kids.", 26.50m, 41),
            ("Magnetic Tile Set", "Transparent magnetic tiles for STEM play and structures.", 45.00m, 33),
            ("Classic Board Game", "Family strategy game with bright pieces and quick rounds.", 21.99m, 59),
            ("Mini Art Easel", "Tabletop easel with chalkboard, whiteboard and supplies.", 30.00m, 29),
            ("Toy Train Starter Set", "Wooden track loop with engine, cars and scenery.", 36.75m, 24),
            ("Bubble Blaster", "Battery-powered bubble maker with refill solution.", 13.99m, 95)
        });
        return products;
    }

    private static void Add(List<Product> products, int categoryId, DateTime now, IEnumerable<(string Name, string Description, decimal Price, int Stock)> items)
    {
        var index = products.Count + 1;
        foreach (var item in items)
        {
            products.Add(new Product
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                StockQuantity = item.Stock,
                CategoryId = categoryId,
                CreatedDate = now.AddDays(-index),
                ImageUrl = $"https://placehold.co/900x700/111111/ffffff?text={Uri.EscapeDataString(item.Name)}"
            });
            index++;
        }
    }
}
