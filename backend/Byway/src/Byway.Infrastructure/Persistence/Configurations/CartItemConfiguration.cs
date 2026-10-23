using Byway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Byway.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        // Settled: a course cannot appear twice in the same cart.
        builder.HasIndex(ci => new { ci.CartId, ci.CourseId }).IsUnique();
    }
}
