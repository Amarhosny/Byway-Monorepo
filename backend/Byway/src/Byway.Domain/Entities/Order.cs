using Byway.Domain.Common;
using Byway.Domain.Enums;

namespace Byway.Domain.Entities;

/// <summary>
/// Persisted as Pending the moment checkout begins, before the simulated
/// payment outcome is known, then updated in place to Paid or Failed.
/// No separate Payment entity and no PaymentMethod field — there is no real
/// payment gateway, so nothing downstream needs either.
/// </summary>
public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal GrandTotal { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
