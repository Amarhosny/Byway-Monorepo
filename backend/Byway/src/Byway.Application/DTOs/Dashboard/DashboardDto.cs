namespace Byway.Application.DTOs.Dashboard;

public class DashboardDto
{
    public int InstructorCount { get; set; }
    public int CategoryCount { get; set; }
    public int CourseCount { get; set; }

    /// <summary>Count of OrderItem rows on Paid orders created this calendar month.</summary>
    public int EnrollmentsThisMonth { get; set; }
}
