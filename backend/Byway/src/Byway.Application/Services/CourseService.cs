using Byway.Application.Common;
using Byway.Application.DTOs.Course;
using Byway.Application.DTOs.Review;
using Byway.Application.Exceptions;
using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Domain.Enums;

namespace Byway.Application.Services;

public class CourseService : ICourseService
{
    private const int RelatedCoursesCount = 4;

    private readonly ICourseRepository _courses;
    private readonly IUnitOfWork _uow;

    public CourseService(ICourseRepository courses, IUnitOfWork uow)
    {
        _courses = courses;
        _uow = uow;
    }

    public async Task<PagedResult<CourseSummaryDto>> GetPagedAsync(CourseFilter filter, PaginationParams pagination, CancellationToken ct = default)
    {
        var result = await _courses.GetPagedAsync(filter, pagination, ct);
        var items = result.Items.Select(ToSummaryDto).ToList();
        return PagedResult<CourseSummaryDto>.Create(items, result.TotalCount, result.PageNumber, result.PageSize);
    }

    public async Task<IReadOnlyList<CourseSummaryDto>> GetTopAsync(int count, CancellationToken ct = default)
    {
        var courses = await _courses.GetTopAsync(count, ct);
        return courses.Select(ToSummaryDto).ToList();
    }

    public async Task<CourseDetailsDto> GetDetailsAsync(int id, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Course), id);

        var related = await _courses.GetRelatedAsync(course.CategoryId, course.Id, RelatedCoursesCount, ct);

        return new CourseDetailsDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Certification = course.Certification,
            CategoryId = course.CategoryId,
            CategoryName = course.Category.Name,
            InstructorId = course.InstructorId,
            InstructorName = course.Instructor.Name,
            InstructorJobTitle = course.Instructor.JobTitle.ToString(),
            InstructorRating = course.Instructor.Rating,
            InstructorDescription = course.Instructor.Description,
            InstructorImagePath = course.Instructor.ImagePath,
            Level = course.Level.ToString(),
            Cost = course.Cost,
            TotalHours = course.TotalHours,
            ImagePath = course.ImagePath,
            Rating = AverageRating(course.Reviews),
            ReviewCount = course.Reviews.Count,
            Contents = course.Contents
                .OrderBy(c => c.SortOrder)
                .Select(c => new CourseContentDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    LecturesNumber = c.LecturesNumber,
                    Time = c.Time,
                    SortOrder = c.SortOrder
                }).ToList(),
            Reviews = course.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserDisplayName = "Student", // resolved to a real name at the API layer if/when a user-lookup service is added
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }).ToList(),
            RelatedCourses = related.Select(ToSummaryDto).ToList()
        };
    }

    public async Task<CourseSummaryDto> CreateAsync(UpsertCourseRequest request, CancellationToken ct = default)
    {
        var course = new Course
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            InstructorId = request.InstructorId,
            Level = Enum.Parse<Level>(request.Level, ignoreCase: true),
            Cost = request.Cost,
            TotalHours = request.TotalHours,
            Description = request.Description,
            Certification = request.Certification,
            ImagePath = request.ImagePath,
            CreatedAt = DateTime.UtcNow,
            Contents = request.Contents.Select(c => new CourseContent
            {
                Name = c.Name,
                LecturesNumber = c.LecturesNumber,
                Time = c.Time,
                SortOrder = c.SortOrder
            }).ToList()
        };

        _courses.Add(course);
        await _uow.SaveChangesAsync(ct);
        return await GetSummaryAfterSaveAsync(course.Id, ct);
    }

    public async Task<CourseSummaryDto> UpdateAsync(int id, UpsertCourseRequest request, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Course), id);

        // Business rule: cannot update a course once it has a Paid purchase.
        // Only Paid counts — Pending/Failed/Cancelled orders do not block editing.
        if (await _courses.HasPaidPurchasesAsync(id, ct))
            throw new ConflictException("This course cannot be edited because it has already been purchased by at least one student.");

        course.Name = request.Name;
        course.CategoryId = request.CategoryId;
        course.InstructorId = request.InstructorId;
        course.Level = Enum.Parse<Level>(request.Level, ignoreCase: true);
        course.Cost = request.Cost;
        course.TotalHours = request.TotalHours;
        course.Description = request.Description;
        course.Certification = request.Certification;
        course.ImagePath = request.ImagePath;

        // Simplest correct approach for content replacement: clear and re-add.
        // Fine at this project's scale; avoids a more complex diff/merge for
        // marginal benefit.
        course.Contents.Clear();
        foreach (var c in request.Contents)
        {
            course.Contents.Add(new CourseContent
            {
                Name = c.Name,
                LecturesNumber = c.LecturesNumber,
                Time = c.Time,
                SortOrder = c.SortOrder
            });
        }

        _courses.Update(course);
        await _uow.SaveChangesAsync(ct);
        return ToSummaryDto(course);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Course), id);

        // Same rule as UpdateAsync — only a Paid purchase blocks deletion.
        if (await _courses.HasPaidPurchasesAsync(id, ct))
            throw new ConflictException("This course cannot be deleted because it has already been purchased by at least one student.");

        _courses.Remove(course);
        await _uow.SaveChangesAsync(ct);
    }

    private async Task<CourseSummaryDto> GetSummaryAfterSaveAsync(int id, CancellationToken ct)
    {
        var course = await _courses.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Course), id);
        return ToSummaryDto(course);
    }

    private static decimal? AverageRating(ICollection<Review> reviews) =>
        reviews.Count == 0 ? null : Math.Round((decimal)reviews.Average(r => r.Rating), 1);

    private static CourseSummaryDto ToSummaryDto(Course c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        CategoryName = c.Category?.Name ?? string.Empty,
        InstructorName = c.Instructor?.Name ?? string.Empty,
        Level = c.Level.ToString(),
        Cost = c.Cost,
        TotalHours = c.TotalHours,
        ImagePath = c.ImagePath,
        Rating = AverageRating(c.Reviews),
        ReviewCount = c.Reviews?.Count ?? 0
    };
}
