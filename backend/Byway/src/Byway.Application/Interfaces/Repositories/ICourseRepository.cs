using Byway.Application.Common;
using Byway.Domain.Entities;

namespace Byway.Application.Interfaces.Repositories;

public class CourseFilter
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Level { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public string? SortBy { get; set; } // "price" | "rating" | "latest"
}

/// <summary>
/// Not a generic CRUD wrapper — every method here hides real query
/// construction (filtering/sorting on a COMPUTED rating average, eager
/// loading, "related courses" logic) that Application should not have to
/// express directly against EF Core.
/// </summary>
public interface ICourseRepository
{
    Task<PagedResult<Course>> GetPagedAsync(CourseFilter filter, PaginationParams pagination, CancellationToken ct = default);

    /// <summary>Includes Category, Instructor, Contents and Reviews.</summary>
    Task<Course?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);

    Task<Course?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Same category as the given course, excluding it, newest first, limited to `count`.</summary>
    Task<IReadOnlyList<Course>> GetRelatedAsync(int categoryId, int excludeCourseId, int count, CancellationToken ct = default);

    Task<IReadOnlyList<Course>> GetTopAsync(int count, CancellationToken ct = default);

    /// <summary>Computed AVG(Review.Rating); returns null when there are zero reviews.</summary>
    Task<decimal?> GetAverageRatingAsync(int courseId, CancellationToken ct = default);

    /// <summary>True if the course is referenced by any OrderItem whose Order.OrderStatus == Paid.</summary>
    Task<bool> HasPaidPurchasesAsync(int courseId, CancellationToken ct = default);

    Task<int> GetTotalCountAsync(CancellationToken ct = default);

    void Add(Course course);
    void Update(Course course);
    void Remove(Course course);
}
