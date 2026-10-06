using System.ComponentModel.DataAnnotations;

namespace Byway.Application.DTOs.Instructor;

/// <summary>Used for both Create and Update — admin form has the same shape for both.</summary>
public class UpsertInstructorRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string JobTitle { get; set; } = string.Empty; // parsed to enum in the service

    [Range(0, 5)]
    public decimal Rating { get; set; }

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Image is uploaded separately via the file endpoint; this carries the resulting path.</summary>
    [Required]
    public string ImagePath { get; set; } = string.Empty;
}
