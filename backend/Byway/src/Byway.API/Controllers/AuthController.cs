using Byway.Application.DTOs.Auth;
using Byway.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Registers a new Student account. Per the bonus requirement, this
    /// also auto-logs-in — the response already contains a usable JWT, so the
    /// frontend does not need a separate login call right after registering.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request, ct);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        return Ok(result);
    }
}
