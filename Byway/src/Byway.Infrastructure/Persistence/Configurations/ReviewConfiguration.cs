using Byway.Domain.Entities;
using Byway.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Byway.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.Comment).HasMaxLength(2000);

        // Settled decision: one review per (UserId, CourseId) — a second
        // submission updates the existing row, enforced at the Application
        // layer; this unique index is the DB-level backstop.
        builder.HasIndex(r => new { r.CourseId, r.UserId }).IsUnique();

        // Shadow FK to AspNetUsers — Domain has no navigation property to
        // ApplicationUser (kept framework-free), but the DB relationship
        // and referential integrity still need to exist.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
