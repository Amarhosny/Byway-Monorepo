using Byway.Application.DTOs.Category;

namespace Byway.Application.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetTopAsync(int count, CancellationToken ct = default);
}
