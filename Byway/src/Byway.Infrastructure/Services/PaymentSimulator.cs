using Byway.Application.Services;

namespace Byway.Infrastructure.Services;

/// <summary>
/// No real payment gateway, per requirement. This is intentionally the only
/// place "payment logic" lives — swap this implementation for a real
/// gateway call later without touching CheckoutService at all.
/// </summary>
public class PaymentSimulator : IPaymentSimulator
{
    private static readonly Random _random = new();

    /// <summary>90% success rate — arbitrary but realistic enough to exercise both the Paid and Failed paths during testing/demo.</summary>
    public bool Simulate() => _random.NextDouble() < 0.9;
}
