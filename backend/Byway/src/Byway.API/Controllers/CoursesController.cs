using Byway.Application.Common;
using Byway.Application.DTOs.Course;
using Byway.Application.Interfaces.Repositories;
using Byway.Application.Services;
using Byway.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    public CoursesController(ICourseService courseService) => _courseService = courseService;

    /// <summary>Public catalog — search/sort/filter/pagination all required per the spec.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaged([FromQuery] CourseFilter filter, [FromQuery] PaginationParams pagination, CancellationToken ct) =>
        Ok(await _courseService.GetPagedAsync(filter, pagination, ct));

    [HttpGet("top")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTop([FromQuery] int count = 4, CancellationToken ct = default) =>
        Ok(await _courseService.GetTopAsync(count, ct));

    /// <summary>Public — full details including Content, Reviews, and the first 4 related courses from the same category.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDetails(int id, CancellationToken ct) =>
        Ok(await _courseService.GetDetailsAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> Create(UpsertCourseRequest request, CancellationToken ct)
    {
        var result = await _courseService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetDetails), new { id = result.Id }, result);
    }

    /// <summary>Blocked by CourseService with 409 if the course has any Paid purchase.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> Update(int id, UpsertCourseRequest request, CancellationToken ct) =>
        Ok(await _courseService.UpdateAsync(id, request, ct));

    /// <summary>Blocked by CourseService with 409 if the course has any Paid purchase.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _courseService.DeleteAsync(id, ct);
        return NoContent();
    }
}
