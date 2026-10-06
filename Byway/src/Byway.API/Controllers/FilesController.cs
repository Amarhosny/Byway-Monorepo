using Byway.Application.Interfaces.Services;
using Byway.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

/// <summary>
/// Admin-only image upload. Returns the string path to store on the
/// entity (Course.ImagePath / Instructor.ImagePath) — per the requirement,
/// the DB never stores binary/base64, only this path.
/// </summary>
[ApiController]
[Route("api/files")]
[Authorize(Roles = IdentityRoles.Admin)]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorage;
    public FilesController(IFileStorageService fileStorage) => _fileStorage = fileStorage;

    private static readonly string[] AllowedContainers = { "courses", "instructors" };

    [HttpPost("upload")]
    [RequestSizeLimit(5_000_000)] // 5 MB
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string container, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        if (!AllowedContainers.Contains(container))
            return BadRequest(new { error = $"Unknown container '{container}'." });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension is not (".jpg" or ".jpeg" or ".png" or ".gif"))
            return BadRequest(new { error = "Only .jpg, .jpeg, .png, or .gif files are allowed." });

        await using var stream = file.OpenReadStream();
        var path = await _fileStorage.SaveAsync(stream, file.FileName, container, ct);
        return Ok(new { path });
    }
}
