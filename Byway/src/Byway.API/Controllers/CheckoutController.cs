using Byway.Application.DTOs.Order;
using Byway.Application.Interfaces.Services;
using Byway.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byway.API.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly ICurrentUserService _currentUser;

    public CheckoutController(ICheckoutService checkoutService, ICurrentUserService currentUser)
    {
        _checkoutService = checkoutService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Creates the Order as Pending, simulates payment, resolves to Paid
    /// (clears cart) or Failed (cart preserved so the user can retry) — see
    /// CheckoutService for the full flow. The response's OrderStatus tells
    /// the frontend which screen to show next (Payment Success vs a retry
    /// prompt).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Checkout(CheckoutRequest request, CancellationToken ct) =>
        Ok(await _checkoutService.CheckoutAsync(_currentUser.UserId!, request, ct));
}
