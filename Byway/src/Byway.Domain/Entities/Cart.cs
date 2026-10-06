using Byway.Domain.Common;

namespace Byway.Domain.Entities;

/// <summary>
/// One Cart per user — created lazily on first add-to-cart, not at
/// registration. Kept as an explicit aggregate root (rather than a bare
/// CartItem(UserId, CourseId) table) for symmetry with Order/OrderItem and
/// to keep the "one cart per user" invariant explicit in the model.
/// </summary>
public class Cart : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
