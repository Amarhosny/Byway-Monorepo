namespace Byway.Application.DTOs.Cart;

public class CartItemDto
{
    public int CartItemId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? Rating { get; set; }
    public int TotalHours { get; set; }
}
