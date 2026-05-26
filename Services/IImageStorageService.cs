namespace StationaryHub.Services;

public interface IImageStorageService
{
    Task<string?> SaveProductImageAsync(IFormFile? file);
}
