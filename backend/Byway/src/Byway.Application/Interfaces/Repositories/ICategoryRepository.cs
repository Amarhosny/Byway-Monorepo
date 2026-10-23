using Byway.Domain.Entities;

namespace Byway.Application.Interfaces.Repositories;

/// <summary>
/// Deliberately read-only and thin — Category is seed-only per the
/// confirmed requirements (no admin CRUD screen exists for it).
/// </summary>
public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetTopAsync(int count, CancellationToken ct = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
}
