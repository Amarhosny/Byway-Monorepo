using Byway.Application.DTOs.Order;
using Byway.Application.Exceptions;
using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Domain.Enums;

namespace Byway.Application.Services;

/// <summary>
/// Implements the confirmed checkout flow:
///   Create Order + OrderItems, persist as Pending (first SaveChanges)
///   -> simulate payment
///   -> Paid: clear cart, persist (second SaveChanges)
///   -> Failed: cart preserved, persist (second SaveChanges)
///
/// This is deliberately two commit points, not one wrapping transaction —
/// Pending must be genuinely observable as a persisted state before the
/// simulated outcome is known, per the confirmed lifecycle.
/// </summary>
public class CheckoutService : ICheckoutService
{
    private const decimal TaxRate = 0.15m;

    private readonly ICartRepository _carts;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IPaymentSimulator _paymentSimulator;

    public CheckoutService(ICartRepository carts, IOrderRepository orders, IUnitOfWork uow, IPaymentSimulator paymentSimulator)
    {
        _carts = carts;
        _orders = orders;
        _uow = uow;
        _paymentSimulator = paymentSimulator;
    }

    public async Task<OrderDto> CheckoutAsync(string userId, CheckoutRequest request, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct);
        if (cart is null || cart.Items.Count == 0)
            throw new ConflictException("Your cart is empty.");

        var subTotal = cart.Items.Sum(i => i.Course.Cost);
        var tax = Math.Round(subTotal * TaxRate, 2);

        var order = new Order
        {
            UserId = userId,
            SubTotal = subTotal,
            Tax = tax,
            GrandTotal = subTotal + tax,
            OrderStatus = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = cart.Items.Select(i => new OrderItem
            {
                CourseId = i.CourseId,
                PriceAtPurchase = i.Course.Cost
            }).ToList()
        };

        _orders.Add(order);
        await _uow.SaveChangesAsync(ct); // Pending is now genuinely persisted

        var succeeded = _paymentSimulator.Simulate();

        if (succeeded)
        {
            order.OrderStatus = OrderStatus.Paid;
            order.UpdatedAt = DateTime.UtcNow;
            _carts.RemoveItems(cart.Items);
        }
        else
        {
            order.OrderStatus = OrderStatus.Failed;
            order.UpdatedAt = DateTime.UtcNow;
            // Cart is deliberately left untouched so the user can retry.
        }

        await _uow.SaveChangesAsync(ct);

        return new OrderDto
        {
            Id = order.Id,
            OrderStatus = order.OrderStatus.ToString(),
            SubTotal = order.SubTotal,
            Tax = order.Tax,
            GrandTotal = order.GrandTotal,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                CourseId = i.CourseId,
                CourseName = cart.Items.First(ci => ci.CourseId == i.CourseId).Course.Name,
                PriceAtPurchase = i.PriceAtPurchase
            }).ToList()
        };
    }
}

/// <summary>
/// Isolated so the "always succeed" vs "randomly fail" behavior is a one-line
/// swap, and so it can be mocked in tests instead of being random.
/// </summary>
public interface IPaymentSimulator
{
    bool Simulate();
}
