using Byway.Application.DTOs.Auth;
using Byway.Application.Exceptions;
using Byway.Application.Interfaces.Services;
using Byway.Application.Services;
using Byway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Byway.Infrastructure.Services;

/// <summary>
/// Real IAuthService implementation. Lives in Infrastructure (not
/// Application) because it depends on UserManager/RoleManager from ASP.NET
/// Core Identity — Application must not reference Identity directly.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator tokenGenerator,
        IEmailService emailService)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new ConflictException("An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ConflictException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, IdentityRoles.Student);

        // Bonus feature — failure to send must never fail registration itself.
        await _emailService.SendAsync(
            user.Email!,
            "Welcome aboard!",
            $"Welcome aboard, {user.FirstName}! 🎉 Let's grow your skills together, your learning journey starts here 🎓",
            ct);

        // Bonus feature: auto-login after registration — issue a token
        // immediately rather than requiring a separate login call.
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenGenerator.GenerateToken(user.Id, user.Email!, roles);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            Roles = roles.ToList()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenGenerator.GenerateToken(user.Id, user.Email!, roles);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            Roles = roles.ToList()
        };
    }
}
