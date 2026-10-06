using Byway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Byway.Infrastructure.Persistence.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(i => i.Name);

        builder.Property(i => i.Rating).HasColumnType("decimal(3,2)");
        builder.Property(i => i.Description).IsRequired().HasMaxLength(2000);
        builder.Property(i => i.ImagePath).IsRequired().HasMaxLength(500);
    }
}
