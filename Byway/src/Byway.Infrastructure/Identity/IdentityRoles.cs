namespace Byway.Infrastructure.Identity;

/// <summary>Real ASP.NET Core Identity roles — not a custom claim/bool, per the settled decision.</summary>
public static class IdentityRoles
{
    public const string Admin = "Admin";
    public const string Student = "Student";
}
