namespace Byway.Application.DTOs.Course;

/// <summary>Shape used by list/grid views (landing top-courses, catalog, admin grid) — NOT the same shape as details.</summary>
public class CourseSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int TotalHours { get; set; }
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>Computed average; null when the course has zero reviews.</summary>
    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }
}
