using Byway.Application.DTOs.Review;

namespace Byway.Application.Services;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);
    Task<ReviewDto> UpsertAsync(string userId, int courseId, UpsertReviewRequest request, CancellationToken ct = default);
}
