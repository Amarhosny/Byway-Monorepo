using Byway.Application.Interfaces.Services;
using Byway.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

/// <summary>All endpoints require login, per the confirmed requirement — the cart is server-side and tied to a real account.</summary>
[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ICurrentUserService _currentUser;

    public CartController(ICartService cartService, ICurrentUserService currentUser)
    {
        _cartService = cartService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct) =>
        Ok(await _cartService.GetCartAsync(_currentUser.UserId!, ct));

    /// <summary>Blocked with 409 if already in cart, or already purchased (Paid).</summary>
    [HttpPost("items/{courseId:int}")]
    public async Task<IActionResult> AddItem(int courseId, CancellationToken ct) =>
        Ok(await _cartService.AddItemAsync(_currentUser.UserId!, courseId, ct));

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartItemId, CancellationToken ct) =>
        Ok(await _cartService.RemoveItemAsync(_currentUser.UserId!, cartItemId, ct));
}
