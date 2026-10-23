using Byway.Domain.Entities;

namespace Byway.Application.Interfaces.Repositories;

public interface ICartRepository
{
    /// <summary>Includes Items.Course. Does NOT create a cart if none exists — see CartService for lazy creation.</summary>
    Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default);

    void Add(Cart cart);

    Task<bool> HasItemAsync(int cartId, int courseId, CancellationToken ct = default);

    void RemoveItem(CartItem item);
    void RemoveItems(IEnumerable<CartItem> items);
}
