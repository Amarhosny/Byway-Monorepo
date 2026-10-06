using Byway.Application.Common;
using Byway.Application.DTOs.Instructor;
using Byway.Application.Exceptions;
using Byway.Application.Interfaces.Repositories;
using Byway.Domain.Entities;
using Byway.Domain.Enums;

namespace Byway.Application.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructors;
    private readonly IUnitOfWork _uow;

    public InstructorService(IInstructorRepository instructors, IUnitOfWork uow)
    {
        _instructors = instructors;
        _uow = uow;
    }

    public async Task<PagedResult<InstructorDto>> GetPagedAsync(string? search, PaginationParams pagination, CancellationToken ct = default)
    {
        var result = await _instructors.GetPagedAsync(search, pagination, ct);
        var items = result.Items.Select(ToDto).ToList();
        return PagedResult<InstructorDto>.Create(items, result.TotalCount, result.PageNumber, result.PageSize);
    }

    public async Task<IReadOnlyList<InstructorDto>> GetTopAsync(int count, CancellationToken ct = default)
    {
        var instructors = await _instructors.GetTopAsync(count, ct);
        return instructors.Select(ToDto).ToList();
    }

    public async Task<InstructorDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var instructor = await _instructors.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Instructor), id);
        return ToDto(instructor);
    }

    public async Task<InstructorDto> CreateAsync(UpsertInstructorRequest request, CancellationToken ct = default)
    {
        var instructor = new Instructor
        {
            Name = request.Name,
            JobTitle = Enum.Parse<JobTitle>(request.JobTitle, ignoreCase: true),
            Rating = request.Rating,
            Description = request.Description,
            ImagePath = request.ImagePath
        };

        _instructors.Add(instructor);
        await _uow.SaveChangesAsync(ct);
        return ToDto(instructor);
    }

    public async Task<InstructorDto> UpdateAsync(int id, UpsertInstructorRequest request, CancellationToken ct = default)
    {
        var instructor = await _instructors.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Instructor), id);

        instructor.Name = request.Name;
        instructor.JobTitle = Enum.Parse<JobTitle>(request.JobTitle, ignoreCase: true);
        instructor.Rating = request.Rating;
        instructor.Description = request.Description;
        instructor.ImagePath = request.ImagePath;

        _instructors.Update(instructor);
        await _uow.SaveChangesAsync(ct);
        return ToDto(instructor);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var instructor = await _instructors.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Instructor), id);

        // Business rule: cannot delete an instructor assigned to any course.
        // Lives here (Application), not as a bare DB FK restrict, so the
        // caller gets a clean 409 instead of a raw database error.
        if (await _instructors.HasCoursesAsync(id, ct))
            throw new ConflictException("This instructor cannot be deleted because they are still assigned to one or more courses.");

        _instructors.Remove(instructor);
        await _uow.SaveChangesAsync(ct);
    }

    private static InstructorDto ToDto(Instructor i) => new()
    {
        Id = i.Id,
        Name = i.Name,
        JobTitle = i.JobTitle.ToString(),
        Rating = i.Rating,
        Description = i.Description,
        ImagePath = i.ImagePath
    };
}
