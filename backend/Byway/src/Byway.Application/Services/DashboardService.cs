using Byway.Application.DTOs.Dashboard;
using Byway.Application.Interfaces.Repositories;

namespace Byway.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ICourseRepository _courses;
    private readonly ICategoryRepository _categories;
    private readonly IInstructorRepository _instructors;
    private readonly IOrderRepository _orders;

    public DashboardService(
        ICourseRepository courses,
        ICategoryRepository categories,
        IInstructorRepository instructors,
        IOrderRepository orders)
    {
        _courses = courses;
        _categories = categories;
        _instructors = instructors;
        _orders = orders;
    }

    public async Task<DashboardDto> GetAsync(CancellationToken ct = default) => new()
    {
        InstructorCount = await _instructors.GetTotalCountAsync(ct),
        CategoryCount = await _categories.GetTotalCountAsync(ct),
        CourseCount = await _courses.GetTotalCountAsync(ct),
        EnrollmentsThisMonth = await _orders.GetEnrollmentsThisMonthAsync(ct)
    };
}
