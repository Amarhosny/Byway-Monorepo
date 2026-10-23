using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Byway.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;
    public CartRepository(AppDbContext db) => _db = db;

    public Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Course).ThenInclude(course => course.Instructor)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public void Add(Cart cart) => _db.Carts.Add(cart);

    public Task<bool> HasItemAsync(int cartId, int courseId, CancellationToken ct = default) =>
        _db.CartItems.AnyAsync(i => i.CartId == cartId && i.CourseId == courseId, ct);

    public void RemoveItem(CartItem item) => _db.CartItems.Remove(item);

    public void RemoveItems(IEnumerable<CartItem> items) => _db.CartItems.RemoveRange(items);
}
