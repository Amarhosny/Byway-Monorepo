using Byway.Application.Common;
using Byway.Application.DTOs.Course;
using Byway.Application.Interfaces.Repositories;

namespace Byway.Application.Services;

public interface ICourseService
{
    Task<PagedResult<CourseSummaryDto>> GetPagedAsync(CourseFilter filter, PaginationParams pagination, CancellationToken ct = default);
    Task<IReadOnlyList<CourseSummaryDto>> GetTopAsync(int count, CancellationToken ct = default);
    Task<CourseDetailsDto> GetDetailsAsync(int id, CancellationToken ct = default);
    Task<CourseSummaryDto> CreateAsync(UpsertCourseRequest request, CancellationToken ct = default);
    Task<CourseSummaryDto> UpdateAsync(int id, UpsertCourseRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
