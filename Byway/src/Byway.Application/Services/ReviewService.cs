using Byway.Application.DTOs.Review;
using Byway.Application.Exceptions;
using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;

namespace Byway.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviews;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public ReviewService(IReviewRepository reviews, IOrderRepository orders, IUnitOfWork uow)
    {
        _reviews = reviews;
        _orders = orders;
        _uow = uow;
    }

    public async Task<IReadOnlyList<ReviewDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var reviews = await _reviews.GetByCourseIdAsync(courseId, ct);
        return reviews.Select(ToDto).ToList();
    }

    public async Task<ReviewDto> UpsertAsync(string userId, int courseId, UpsertReviewRequest request, CancellationToken ct = default)
    {
        // Business rule: must have a Paid order containing this course.
        // Same shared "has purchased" definition used by the cart-add guard
        // and the course edit/delete guard.
        if (!await _orders.HasUserPurchasedCourseAsync(userId, courseId, ct))
            throw new ForbiddenException("You can only review courses you have purchased.");

        var existing = await _reviews.GetByUserAndCourseAsync(userId, courseId, ct);

        if (existing is null)
        {
            var review = new Review
            {
                CourseId = courseId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };
            _reviews.Add(review);
            await _uow.SaveChangesAsync(ct);
            return ToDto(review);
        }

        // One review per (UserId, CourseId) — a second submission updates
        // the existing row rather than creating a new one.
        existing.Rating = request.Rating;
        existing.Comment = request.Comment;
        existing.UpdatedAt = DateTime.UtcNow;
        _reviews.Update(existing);
        await _uow.SaveChangesAsync(ct);
        return ToDto(existing);
    }

    private static ReviewDto ToDto(Review r) => new()
    {
        Id = r.Id,
        UserId = r.UserId,
        UserDisplayName = "Student",
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
