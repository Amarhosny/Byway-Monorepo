using Byway.Application.Common;
using Byway.Domain.Entities;

namespace Byway.Application.Interfaces.Repositories;

public interface IInstructorRepository
{
    Task<PagedResult<Instructor>> GetPagedAsync(string? search, PaginationParams pagination, CancellationToken ct = default);
    Task<Instructor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Instructor>> GetTopAsync(int count, CancellationToken ct = default);

    /// <summary>Delete guard — true if any Course currently references this instructor.</summary>
    Task<bool> HasCoursesAsync(int instructorId, CancellationToken ct = default);

    Task<int> GetTotalCountAsync(CancellationToken ct = default);

    void Add(Instructor instructor);
    void Update(Instructor instructor);
    void Remove(Instructor instructor);
}
