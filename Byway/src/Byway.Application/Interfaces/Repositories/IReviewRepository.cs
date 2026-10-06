using Byway.Domain.Entities;

namespace Byway.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);
    Task<Review?> GetByUserAndCourseAsync(string userId, int courseId, CancellationToken ct = default);
    void Add(Review review);
    void Update(Review review);
}
