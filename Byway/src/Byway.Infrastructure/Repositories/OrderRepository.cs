using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Domain.Enums;
using Byway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Byway.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public OrderRepository(AppDbContext db) => _db = db;

    public void Add(Order order) => _db.Orders.Add(order);

    public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Orders.Include(o => o.Items).ThenInclude(i => i.Course).FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Course)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    /// <summary>
    /// The single shared definition of "has purchased" — used by the
    /// cart-add guard, the review-eligibility check, and (indirectly, via
    /// CourseRepository.HasPaidPurchasesAsync) the course edit/delete guard.
    /// Deliberately not duplicated anywhere else.
    /// </summary>
    public Task<bool> HasUserPurchasedCourseAsync(string userId, int courseId, CancellationToken ct = default) =>
        _db.OrderItems.AnyAsync(oi =>
            oi.CourseId == courseId &&
            oi.Order.UserId == userId &&
            oi.Order.OrderStatus == OrderStatus.Paid, ct);

    public Task<int> GetEnrollmentsThisMonthAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        return _db.OrderItems.CountAsync(oi =>
            oi.Order.OrderStatus == OrderStatus.Paid &&
            oi.Order.CreatedAt >= monthStart, ct);
    }
}
