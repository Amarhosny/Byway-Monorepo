using Byway.Domain.Entities;
using Byway.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Byway.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.UserId).IsRequired();
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.CreatedAt); // supports the "enrollments this month" dashboard query

        builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Tax).HasColumnType("decimal(18,2)");
        builder.Property(o => o.GrandTotal).HasColumnType("decimal(18,2)");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // deleting an order legitimately deletes its line items
    }
}
