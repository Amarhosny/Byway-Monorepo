using Byway.Application.DTOs.Cart;
using Byway.Application.Exceptions;
using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;

namespace Byway.Application.Services;

public class CartService : ICartService
{
    private const decimal TaxRate = 0.15m;

    private readonly ICartRepository _carts;
    private readonly ICourseRepository _courses;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public CartService(ICartRepository carts, ICourseRepository courses, IOrderRepository orders, IUnitOfWork uow)
    {
        _carts = carts;
        _courses = courses;
        _orders = orders;
        _uow = uow;
    }

    public async Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct);
        return await ToDtoAsync(cart, ct);
    }

    public async Task<CartDto> AddItemAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(courseId, ct)
            ?? throw new NotFoundException(nameof(Course), courseId);

        // Business rule: a course already covered by a Paid order cannot be
        // re-added to the cart. Uses the single shared "has purchased"
        // definition from IOrderRepository — the same one review-eligibility
        // and course edit/delete rely on.
        if (await _orders.HasUserPurchasedCourseAsync(userId, courseId, ct))
            throw new ConflictException("You have already purchased this course.");

        var cart = await _carts.GetByUserIdAsync(userId, ct);

        // Lazy creation — a cart only comes into existence on first add.
        if (cart is null)
        {
            cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
            _carts.Add(cart);
            await _uow.SaveChangesAsync(ct); // need the Cart.Id before adding items
        }
        else if (await _carts.HasItemAsync(cart.Id, courseId, ct))
        {
            throw new ConflictException("This course is already in your cart.");
        }

        cart.Items.Add(new CartItem { CartId = cart.Id, CourseId = courseId, AddedAt = DateTime.UtcNow });
        await _uow.SaveChangesAsync(ct);

        var refreshed = await _carts.GetByUserIdAsync(userId, ct);
        return await ToDtoAsync(refreshed, ct);
    }

    public async Task<CartDto> RemoveItemAsync(string userId, int cartItemId, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Cart), userId);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new NotFoundException(nameof(CartItem), cartItemId);

        _carts.RemoveItem(item);
        await _uow.SaveChangesAsync(ct);

        var refreshed = await _carts.GetByUserIdAsync(userId, ct);
        return await ToDtoAsync(refreshed, ct);
    }

    private async Task<CartDto> ToDtoAsync(Cart? cart, CancellationToken ct)
    {
        if (cart is null || cart.Items.Count == 0)
            return new CartDto { Items = Array.Empty<CartItemDto>(), SubTotal = 0, Tax = 0, GrandTotal = 0 };

        var items = new List<CartItemDto>();
        foreach (var item in cart.Items)
        {
            var rating = await _courses.GetAverageRatingAsync(item.CourseId, ct);
            items.Add(new CartItemDto
            {
                CartItemId = item.Id,
                CourseId = item.CourseId,
                CourseName = item.Course.Name,
                InstructorName = item.Course.Instructor?.Name ?? string.Empty,
                ImagePath = item.Course.ImagePath,
                Price = item.Course.Cost,
                Rating = rating,
                TotalHours = item.Course.TotalHours
            });
        }

        var subTotal = items.Sum(i => i.Price);
        var tax = Math.Round(subTotal * TaxRate, 2);

        return new CartDto
        {
            Items = items,
            SubTotal = subTotal,
            Tax = tax,
            GrandTotal = subTotal + tax
        };
    }
}
