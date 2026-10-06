using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Byway.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _db;
    public ReviewRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Review>> GetByCourseIdAsync(int courseId, CancellationToken ct = default) =>
        await _db.Reviews
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public Task<Review?> GetByUserAndCourseAsync(string userId, int courseId, CancellationToken ct = default) =>
        _db.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId, ct);

    public void Add(Review review) => _db.Reviews.Add(review);
    public void Update(Review review) => _db.Reviews.Update(review);
}
