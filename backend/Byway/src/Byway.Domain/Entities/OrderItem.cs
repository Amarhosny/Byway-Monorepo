using Byway.Domain.Common;

namespace Byway.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    /// <summary>
    /// Snapshot of Course.Cost at the moment of purchase. Never recalculated
    /// from the current course price — later price changes must not affect
    /// historical order totals.
    /// </summary>
    public decimal PriceAtPurchase { get; set; }
}
