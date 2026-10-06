namespace Byway.Application.Interfaces.Services;

/// <summary>
/// Lets Application code (services) read "who is making this request"
/// without depending on HttpContext directly — Infrastructure/API wires
/// this to the real ClaimsPrincipal.
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
}
