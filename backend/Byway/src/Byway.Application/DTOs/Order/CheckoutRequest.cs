using System.ComponentModel.DataAnnotations;

namespace Byway.Application.DTOs.Order;

/// <summary>
/// Country/State mirror the Figma checkout form fields. They are captured
/// for realism (validations exist per the requirement) but do not affect
/// pricing or business logic — no tax-by-region rule exists in scope.
/// </summary>
public class CheckoutRequest
{
    [Required, MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string State { get; set; } = string.Empty;
}
