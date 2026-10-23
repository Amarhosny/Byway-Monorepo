using Microsoft.AspNetCore.Identity;

namespace Byway.Infrastructure.Identity;

/// <summary>
/// Extends Identity's IdentityUser. Deliberately lives in Infrastructure,
/// NOT Domain — Domain entities (Cart, Order, Review, ...) reference a user
/// only via a plain string UserId, never a navigation property to this
/// class, so Domain never needs a package reference to
/// Microsoft.AspNetCore.Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
