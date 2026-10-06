using Byway.Application.DTOs.Cart;

namespace Byway.Application.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default);
    Task<CartDto> AddItemAsync(string userId, int courseId, CancellationToken ct = default);
    Task<CartDto> RemoveItemAsync(string userId, int cartItemId, CancellationToken ct = default);
}
