using Byway.Domain.Common;

namespace Byway.Domain.Entities;

/// <summary>
/// Seed-only reference data — no admin CRUD exists for Category per the
/// confirmed requirements (only Instructor and Course management are
/// specified as admin modules).
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional — landing page renders categories with a generic icon.</summary>
    public string? ImagePath { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
