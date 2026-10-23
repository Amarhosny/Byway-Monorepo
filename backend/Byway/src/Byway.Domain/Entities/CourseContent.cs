using Byway.Domain.Common;

namespace Byway.Domain.Entities;

/// <summary>
/// A flat curriculum section (e.g. "Introduction to UX Design — 5 lectures,
/// 60 min"). Deliberately NOT a two-level hierarchy — nothing in the
/// confirmed design shows individual lecture titles/links, only a section
/// name + lecture count + time per section.
/// </summary>
public class CourseContent : BaseEntity
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public int LecturesNumber { get; set; }

    /// <summary>Stored as minutes for precision; format for display in the API/frontend.</summary>
    public int Time { get; set; }

    /// <summary>Explicit ordering, independent of Id/insertion order.</summary>
    public int SortOrder { get; set; }
}
