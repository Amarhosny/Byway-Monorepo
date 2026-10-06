namespace Byway.Domain.Enums;

/// <summary>
/// Pending is the initial persisted status, set the moment checkout begins,
/// before the simulated payment result is known. It resolves to Paid or
/// Failed. Cancelled exists in the enum but currently has no defined trigger
/// in the confirmed requirements — do not wire up a path to it without an
/// explicit product decision.
/// </summary>
public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3,
    Cancelled = 4
}
