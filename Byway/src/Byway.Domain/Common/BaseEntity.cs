namespace Byway.Domain.Common;

/// <summary>
/// Base class for every Domain entity. Deliberately minimal — Domain stays
/// framework-free (no EF Core, no Identity references here). ApplicationUser
/// itself lives in Infrastructure (it extends IdentityUser), so every entity
/// below references a user only by string UserId, never a navigation
/// property back to ApplicationUser.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}
