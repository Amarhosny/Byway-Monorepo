using Byway.Domain.Common;

namespace Byway.Domain.Entities;

/// <summary>
/// Unique on (CartId, CourseId) — enforced in Infrastructure — so a course
/// cannot appear twice in the same cart.
/// </summary>
public class CartItem : BaseEntity
{
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public DateTime AddedAt { get; set; }
}
