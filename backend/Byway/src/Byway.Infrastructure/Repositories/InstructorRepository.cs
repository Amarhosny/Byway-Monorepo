using Byway.Application.Common;
using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Byway.Infrastructure.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly AppDbContext _db;
    public InstructorRepository(AppDbContext db) => _db = db;

    public async Task<PagedResult<Instructor>> GetPagedAsync(string? search, PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _db.Instructors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(i =>
                EF.Functions.Like(i.Name, $"%{term}%") ||
                EF.Functions.Like(i.JobTitle.ToString(), $"%{term}%"));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(i => i.Name)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        return PagedResult<Instructor>.Create(items, total, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<IReadOnlyList<Instructor>> GetTopAsync(int count, CancellationToken ct = default) =>
        await _db.Instructors.OrderByDescending(i => i.Rating).Take(count).ToListAsync(ct);

    public Task<Instructor?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Instructors.FirstOrDefaultAsync(i => i.Id == id, ct);

    public Task<bool> HasCoursesAsync(int instructorId, CancellationToken ct = default) =>
        _db.Courses.AnyAsync(c => c.InstructorId == instructorId, ct);

    public Task<int> GetTotalCountAsync(CancellationToken ct = default) => _db.Instructors.CountAsync(ct);

    public void Add(Instructor instructor) => _db.Instructors.Add(instructor);
    public void Update(Instructor instructor) => _db.Instructors.Update(instructor);
    public void Remove(Instructor instructor) => _db.Instructors.Remove(instructor);
}
