using Byway.Application.Common;
using Byway.Application.DTOs.Course;
using Byway.Application.Interfaces.Repositories;
using Byway.Application.Services;
using Byway.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers.Admin;

[ApiController]
[Route("api/admin/courses")]
[Authorize(Roles = IdentityRoles.Admin)]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService) => _courseService = courseService;

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] CourseFilter filter, [FromQuery] PaginationParams pagination, CancellationToken ct) =>
        Ok(await _courseService.GetPagedAsync(filter, pagination, ct));
}
