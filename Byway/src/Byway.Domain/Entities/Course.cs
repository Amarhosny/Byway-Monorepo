using Byway.Domain.Common;
using Byway.Domain.Enums;

namespace Byway.Domain.Entities;

public class Course : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;

    public Level Level { get; set; }
    public decimal Cost { get; set; }
    public int TotalHours { get; set; }

    public string Description { get; set; } = string.Empty;

    /// <summary>Optional — secondary marketing blurb, not structurally load-bearing.</summary>
    public string? Certification { get; set; }

    public string ImagePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // NOTE: Rating is intentionally NOT a stored property here.
    // It is always computed as AVG(Review.Rating) at query time,
    // by the repository/service layer — see ICourseRepository.

    public ICollection<CourseContent> Contents { get; set; } = new List<CourseContent>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
