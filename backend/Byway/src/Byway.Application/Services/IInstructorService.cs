using Byway.Application.Common;
using Byway.Application.DTOs.Instructor;

namespace Byway.Application.Services;

public interface IInstructorService
{
    Task<PagedResult<InstructorDto>> GetPagedAsync(string? search, PaginationParams pagination, CancellationToken ct = default);
    Task<IReadOnlyList<InstructorDto>> GetTopAsync(int count, CancellationToken ct = default);
    Task<InstructorDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<InstructorDto> CreateAsync(UpsertInstructorRequest request, CancellationToken ct = default);
    Task<InstructorDto> UpdateAsync(int id, UpsertInstructorRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
