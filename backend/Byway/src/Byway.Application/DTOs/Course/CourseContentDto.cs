using System.ComponentModel.DataAnnotations;

namespace Byway.Application.DTOs.Course;

public class CourseContentDto
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 500)]
    public int LecturesNumber { get; set; }

    /// <summary>Minutes.</summary>
    [Range(1, 10000)]
    public int Time { get; set; }

    public int SortOrder { get; set; }
}
