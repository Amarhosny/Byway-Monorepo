using Byway.Application.DTOs.Review;
using Byway.Application.Interfaces.Services;
using Byway.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserService _currentUser;

    public ReviewsController(IReviewService reviewService, ICurrentUserService currentUser)
    {
        _reviewService = reviewService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCourse(int courseId, CancellationToken ct) =>
        Ok(await _reviewService.GetByCourseIdAsync(courseId, ct));

    /// <summary>
    /// Requires the caller to have a Paid order containing this course
    /// (enforced in ReviewService, returns 403 otherwise). One review per
    /// user per course — a second call here updates the existing review
    /// rather than creating a new one.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upsert(int courseId, UpsertReviewRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!;
        var result = await _reviewService.UpsertAsync(userId, courseId, request, ct);
        return Ok(result);
    }
}
