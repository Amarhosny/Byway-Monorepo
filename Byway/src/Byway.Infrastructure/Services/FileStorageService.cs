using Byway.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace Byway.Infrastructure.Services;

/// <summary>
/// Local-disk implementation (wwwroot/images/{container}/{fileName}).
/// Stores/returns a string path per the requirement (never binary). Swap in
/// an S3/Azure Blob implementation later by implementing IFileStorageService
/// again — nothing in Application changes.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveAsync(Stream content, string fileName, string containerName, CancellationToken ct = default)
    {
        var safeFileName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        var containerPath = Path.Combine(_env.WebRootPath, "images", containerName);
        Directory.CreateDirectory(containerPath);

        var fullPath = Path.Combine(containerPath, safeFileName);
        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        await content.CopyToAsync(fileStream, ct);

        // Relative path — this is what gets stored on the entity (Course.ImagePath, etc.)
        return $"/images/{containerName}/{safeFileName}";
    }

    public void Delete(string path)
    {
        var relative = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_env.WebRootPath, relative);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
