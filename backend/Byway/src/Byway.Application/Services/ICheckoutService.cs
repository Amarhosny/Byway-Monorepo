using Byway.Application.DTOs.Order;

namespace Byway.Application.Services;

public interface ICheckoutService
{
    Task<OrderDto> CheckoutAsync(string userId, CheckoutRequest request, CancellationToken ct = default);
}
