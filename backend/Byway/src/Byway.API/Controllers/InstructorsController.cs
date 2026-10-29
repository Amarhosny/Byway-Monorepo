using Byway.Application.Common;
using Byway.Application.DTOs.Instructor;
using Byway.Application.Services;
using Byway.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

[ApiController]
[Route("api/instructors")]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;
    public InstructorsController(IInstructorService instructorService) => _instructorService = instructorService;

    /// <summary>Admin-only management listing. The public landing page uses the separate top endpoint.</summary>
    [HttpGet]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> GetPaged([FromQuery] string? search, [FromQuery] PaginationParams pagination, CancellationToken ct) =>
        Ok(await _instructorService.GetPagedAsync(search, pagination, ct));

    [HttpGet("top")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTop([FromQuery] int count = 5, CancellationToken ct = default) =>
        Ok(await _instructorService.GetTopAsync(count, ct));

    [HttpGet("{id:int}")]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await _instructorService.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> Create(UpsertInstructorRequest request, CancellationToken ct)
    {
        var result = await _instructorService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> Update(int id, UpsertInstructorRequest request, CancellationToken ct) =>
        Ok(await _instructorService.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = IdentityRoles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _instructorService.DeleteAsync(id, ct);
        return NoContent();
    }
}
