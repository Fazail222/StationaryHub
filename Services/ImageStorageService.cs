namespace StationaryHub.Services;

public class ImageStorageService : IImageStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private readonly IWebHostEnvironment _environment;

    public ImageStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string?> SaveProductImageAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        if (file.Length > 3 * 1024 * 1024)
        {
            throw new InvalidOperationException("Images must be 3 MB or smaller.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Only JPG, PNG, WEBP and GIF images are allowed.");
        }

        var uploads = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);
        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var path = Path.Combine(uploads, fileName);

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream);
        return $"/uploads/{fileName}";
    }
}
