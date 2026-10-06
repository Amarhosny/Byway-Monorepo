using System.ComponentModel.DataAnnotations;

namespace Byway.Application.DTOs.Course;

public class UpsertCourseRequest
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int InstructorId { get; set; }

    [Required]
    public string Level { get; set; } = string.Empty; // parsed to enum in the service

    [Range(0, 100000)]
    public decimal Cost { get; set; }

    [Range(1, 2000)]
    public int TotalHours { get; set; }

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public string? Certification { get; set; }

    [Required]
    public string ImagePath { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<CourseContentDto> Contents { get; set; } = new();
}
