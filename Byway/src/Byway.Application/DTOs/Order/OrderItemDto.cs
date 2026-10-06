namespace Byway.Application.DTOs.Order;

public class OrderItemDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal PriceAtPurchase { get; set; }
}
