using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Byway.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;
    public CategoryRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Categories.Include(c => c.Courses).OrderBy(c => c.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Category>> GetTopAsync(int count, CancellationToken ct = default) =>
        await _db.Categories
            .Include(c => c.Courses)
            .OrderByDescending(c => c.Courses.Count)
            .Take(count)
            .ToListAsync(ct);

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Categories.Include(c => c.Courses).FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<int> GetTotalCountAsync(CancellationToken ct = default) => _db.Categories.CountAsync(ct);
}
