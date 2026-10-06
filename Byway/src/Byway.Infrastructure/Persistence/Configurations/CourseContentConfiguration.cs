using Byway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Byway.Infrastructure.Persistence.Configurations;

public class CourseContentConfiguration : IEntityTypeConfiguration<CourseContent>
{
    public void Configure(EntityTypeBuilder<CourseContent> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);

        // Supports ordered retrieval of a course's curriculum sections.
        builder.HasIndex(c => new { c.CourseId, c.SortOrder });
    }
}
