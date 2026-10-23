using Byway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Byway.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.HasIndex(c => c.Name);

        builder.Property(c => c.Cost).HasColumnType("decimal(18,2)");
        builder.Property(c => c.Description).IsRequired().HasMaxLength(4000);
        builder.Property(c => c.Certification).HasMaxLength(4000); // optional, per settled nullability decision
        builder.Property(c => c.ImagePath).IsRequired().HasMaxLength(500);

        // Architecture review A1: an Order's OrderItem must never be
        // silently cascade-deleted just because its Course gets deleted.
        // Deletion is already gated at the Application layer (blocked
        // whenever a Paid OrderItem references this course); Restrict here
        // is the DB-level backstop so a bypass can never corrupt an Order's
        // historical totals.
        builder.HasMany(c => c.OrderItems)
            .WithOne(oi => oi.Course)
            .HasForeignKey(oi => oi.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Same precautionary reasoning extended to Review — in practice a
        // Review can only exist if a Paid purchase exists, which already
        // blocks deletion at the Application layer, but Restrict avoids
        // relying on that invariant holding forever.
        builder.HasMany(c => c.Reviews)
            .WithOne(r => r.Course)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Architecture review A2: an unpurchased Course CAN be deleted while
        // sitting in another user's cart. Cascade here is a deliberate,
        // explicit choice (not the implicit default) — the alternative
        // (Restrict) would block deleting a perfectly legitimate,
        // never-purchased course just because a stranger's cart references it.
        builder.HasMany(c => c.CartItems)
            .WithOne(ci => ci.Course)
            .HasForeignKey(ci => ci.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Owned child data — deleting a Course legitimately deletes its own
        // curriculum sections.
        builder.HasMany(c => c.Contents)
            .WithOne(cc => cc.Course)
            .HasForeignKey(cc => cc.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Architecture review B3: explicit Restrict rather than relying on
        // EF's implicit default, so Category/Instructor deletion can never
        // silently cascade into Courses even via a bypass of the
        // Application-layer guard.
        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
