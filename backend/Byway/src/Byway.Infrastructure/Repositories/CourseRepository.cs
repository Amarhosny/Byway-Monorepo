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

        // Apply the computed review average before counting and paging so the
        // result count and page contents agree with the requested filter.

        if (filter.MinRating.HasValue)
        {
            var minRating = (double)filter.MinRating.Value;
            query = query.Where(c => c.Reviews.Any() && c.Reviews.Average(r => (double)r.Rating) >= minRating);
        }

        var total = await query.CountAsync(ct);

        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "price" => query.OrderBy(c => c.Cost),
            "rating" => query.OrderByDescending(c => c.Reviews.Any()
                ? c.Reviews.Average(r => (double)r.Rating)
                : 0d),
            "latest" => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        return PagedResult<Course>.Create(items, total, pagination.PageNumber, pagination.PageSize);
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
