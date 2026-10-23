using Byway.Domain.Common;
using Byway.Domain.Enums;

namespace Byway.Domain.Entities;

public class Instructor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public JobTitle JobTitle { get; set; }

    /// <summary>
    /// Admin-entered and stored directly — there is no instructor review
    /// system in scope, unlike Course.Rating which is always computed.
    /// </summary>
    public decimal Rating { get; set; }

    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
