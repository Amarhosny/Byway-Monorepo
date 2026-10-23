using Byway.Application.DTOs.Review;

namespace Byway.Application.DTOs.Course;

/// <summary>Shape used only by the Course Details page — richer than CourseSummaryDto by design, not a superset trimmed at read time.</summary>
public class CourseDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Certification { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string InstructorJobTitle { get; set; } = string.Empty;
    public decimal InstructorRating { get; set; }
    public string InstructorDescription { get; set; } = string.Empty;
    public string InstructorImagePath { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int TotalHours { get; set; }
    public string ImagePath { get; set; } = string.Empty;

    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }

    public IReadOnlyList<CourseContentDto> Contents { get; set; } = Array.Empty<CourseContentDto>();
    public IReadOnlyList<ReviewDto> Reviews { get; set; } = Array.Empty<ReviewDto>();
    public IReadOnlyList<CourseSummaryDto> RelatedCourses { get; set; } = Array.Empty<CourseSummaryDto>();
}
