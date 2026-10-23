using Byway.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

/// <summary>
/// Read-only, deliberately — Category is seed-only per the confirmed
/// requirements (the admin module spec only lists Instructor and Course
/// management, never Category management).
/// </summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _categoryService.GetAllAsync(ct));

    [HttpGet("top")]
    public async Task<IActionResult> GetTop([FromQuery] int count = 4, CancellationToken ct = default) =>
        Ok(await _categoryService.GetTopAsync(count, ct));
}
