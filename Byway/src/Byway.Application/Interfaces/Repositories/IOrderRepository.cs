using Byway.Domain.Entities;

namespace Byway.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    void Add(Order order);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId, CancellationToken ct = default);

    /// <summary>
    /// The single shared definition of "has this user successfully bought
    /// this course" (Order.OrderStatus == Paid). Deliberately lives here,
    /// not duplicated inside ICartRepository or IReviewRepository, so the
    /// cart-add guard, the review-eligibility check, and the course
    /// edit/delete guard can never silently drift apart.
    /// </summary>
    Task<bool> HasUserPurchasedCourseAsync(string userId, int courseId, CancellationToken ct = default);

    /// <summary>Count of OrderItem rows on Paid orders created this calendar month — dashboard metric.</summary>
    Task<int> GetEnrollmentsThisMonthAsync(CancellationToken ct = default);
}
