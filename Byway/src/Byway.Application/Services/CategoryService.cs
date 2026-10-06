using Byway.Application.DTOs.Category;
using Byway.Application.Interfaces.Repositories;

namespace Byway.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;

    public CategoryService(ICategoryRepository categories) => _categories = categories;

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _categories.GetAllAsync(ct);
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            ImagePath = c.ImagePath,
            CourseCount = c.Courses.Count
        }).ToList();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetTopAsync(int count, CancellationToken ct = default)
    {
        var categories = await _categories.GetTopAsync(count, ct);
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            ImagePath = c.ImagePath,
            CourseCount = c.Courses.Count
        }).ToList();
    }
}
