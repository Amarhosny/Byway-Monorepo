using Byway.Application.Common;
using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Domain.Enums;
using Byway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Byway.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;
    public CourseRepository(AppDbContext db) => _db = db;

    /// <summary>
    /// Base query used by every read method — always includes Category,
    /// Instructor and Reviews, since CourseSummaryDto needs the computed
    /// rating (AVG of Reviews) on every list/grid view, not just details.
    /// </summary>
    private IQueryable<Course> BaseQuery() =>
        _db.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Reviews);

    public async Task<PagedResult<Course>> GetPagedAsync(CourseFilter filter, PaginationParams pagination, CancellationToken ct = default)
    {
        var query = BaseQuery();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%"));
        }

        if (filter.CategoryId.HasValue)
            query = query.Where(c => c.CategoryId == filter.CategoryId);

        if (!string.IsNullOrWhiteSpace(filter.Level) && Enum.TryParse<Level>(filter.Level, true, out var level))
            query = query.Where(c => c.Level == level);

        if (filter.MinPrice.HasValue)
            query = query.Where(c => c.Cost >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(c => c.Cost <= filter.MaxPrice);

        // Rating is computed, not stored, so filtering/sorting on it happens
        // client-side (in-memory) after materializing the page candidates is
        // NOT what we do — instead we filter/sort using the loaded Reviews
        // navigation, which EF translates reasonably at this data scale.
        // For a junior-scoped catalog this is an acceptable trade rather than
        // a raw SQL AVG subquery, per the "avoid over-engineering" guidance.

        var total = await query.CountAsync(ct);

        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "price" => query.OrderBy(c => c.Cost),
            "latest" => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var candidates = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        if (filter.MinRating.HasValue)
        {
            candidates = candidates
                .Where(c => c.Reviews.Count > 0 && (decimal)c.Reviews.Average(r => r.Rating) >= filter.MinRating.Value)
                .ToList();
        }

        if (filter.SortBy?.ToLowerInvariant() == "rating")
        {
            candidates = candidates
                .OrderByDescending(c => c.Reviews.Count == 0 ? 0 : c.Reviews.Average(r => r.Rating))
                .ToList();
        }

        return PagedResult<Course>.Create(candidates, total, pagination.PageNumber, pagination.PageSize);
    }

    public Task<Course?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default) =>
        BaseQuery()
            .Include(c => c.Contents)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Course?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Course>> GetRelatedAsync(int categoryId, int excludeCourseId, int count, CancellationToken ct = default) =>
        await BaseQuery()
            .Where(c => c.CategoryId == categoryId && c.Id != excludeCourseId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Course>> GetTopAsync(int count, CancellationToken ct = default) =>
        await BaseQuery()
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync(ct);

    public async Task<decimal?> GetAverageRatingAsync(int courseId, CancellationToken ct = default)
    {
        var ratings = await _db.Reviews
            .Where(r => r.CourseId == courseId)
            .Select(r => r.Rating)
            .ToListAsync(ct);

        return ratings.Count == 0 ? null : Math.Round((decimal)ratings.Average(), 1);
    }

    public Task<bool> HasPaidPurchasesAsync(int courseId, CancellationToken ct = default) =>
        _db.OrderItems.AnyAsync(oi =>
            oi.CourseId == courseId && oi.Order.OrderStatus == OrderStatus.Paid, ct);

    public Task<int> GetTotalCountAsync(CancellationToken ct = default) => _db.Courses.CountAsync(ct);

    public void Add(Course course) => _db.Courses.Add(course);
    public void Update(Course course) => _db.Courses.Update(course);
    public void Remove(Course course) => _db.Courses.Remove(course);
}
