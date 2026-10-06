using System.ComponentModel.DataAnnotations;

namespace Byway.Application.DTOs.Review;

/// <summary>
/// Same shape handles both create and update — ReviewService decides which
/// happens based on whether a Review already exists for (UserId, CourseId).
/// </summary>
public class UpsertReviewRequest
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }
}
