namespace Byway.Application.Interfaces.Repositories;

/// <summary>
/// Deliberately minimal — its only job is exposing a shared commit boundary
/// for operations that touch more than one repository in one go (most
/// notably Checkout: Order + OrderItems creation, and separately the
/// Cart/CartItem cleanup once payment resolves to Paid). It is NOT a
/// generic Repository&lt;T&gt; factory — repositories are injected directly
/// where needed, backed by the same scoped DbContext this wraps.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
