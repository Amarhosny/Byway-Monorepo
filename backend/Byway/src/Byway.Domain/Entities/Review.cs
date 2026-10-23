using Byway.Domain.Common;

namespace Byway.Domain.Entities;

/// <summary>
/// One Review per (UserId, CourseId) — enforced by a unique constraint in
/// Infrastructure. A user submitting again updates this row rather than
/// creating a new one; see ReviewService in Application.
/// </summary>
public class Review : BaseEntity
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
