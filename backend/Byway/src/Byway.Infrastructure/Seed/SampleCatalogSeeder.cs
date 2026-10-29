using Byway.Domain.Entities;
using Byway.Domain.Enums;
using Byway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Byway.Infrastructure.Seed;

/// <summary>
/// Migrates the frontend's repeated sample catalog entries into the database.
/// The old mock arrays generated multiple rows from identical records, so the
/// repeated records are reconciled by natural-key group and desired count.
/// Existing rows are never updated or deleted.
/// </summary>
public static class SampleCatalogSeeder
{
    private const int SampleCourseCopies = 9;
    private const int SampleJasonFahdCopies = 7;

    private static readonly string[] CategoryNames =
    {
        "Fullstack",
        "Backend",
        "Frontend",
        "UX/UI Design"
    };

    private static readonly (string Name, int Lectures, int Minutes)[] CourseSections =
    {
        ("Introduction to UX Design", 5, 60),
        ("Basics of User-Centered Design", 5, 60),
        ("Elements of User Experience", 5, 60),
        ("Visual Design Principles", 5, 60)
    };

    private const string CourseName = "Beginner's Guide to Design";
    private const string CourseDescription = "This interactive e-learning course will introduce you to User Experience (UX) design, the art of creating products and services that are intuitive, enjoyable, and user-friendly.";
    private const string InstructorDescription = "With over a decade of industry experience, bringing a wealth of practical knowledge to the classroom.";

    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        var categories = await SeedCategoriesAsync(db, ct);
        var instructors = await SeedInstructorsAsync(db, ct);
        await SeedCoursesAsync(
            db,
            categories["UX/UI Design"],
            instructors.First(i => i.Name == "Ronald Richards"),
            ct);
    }

    private static async Task<Dictionary<string, Category>> SeedCategoriesAsync(AppDbContext db, CancellationToken ct)
    {
        var categories = await db.Categories.ToListAsync(ct);
        foreach (var name in CategoryNames)
        {
            if (categories.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
                continue;

            // Category image assets were not present in the frontend mock data.
            var category = new Category { Name = name, ImagePath = null };
            db.Categories.Add(category);
            categories.Add(category);
        }

        await db.SaveChangesAsync(ct);
        return CategoryNames.ToDictionary(
            name => name,
            name => categories.First(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)),
            StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<List<Instructor>> SeedInstructorsAsync(AppDbContext db, CancellationToken ct)
    {
        var instructors = await db.Instructors.ToListAsync(ct);

        // Seven generated mock entries have the same name, title and rating.
        // Preserve the visible sample count without relying on or reserving DB IDs.
        var jasonCount = instructors.Count(i =>
            i.Name == "Jason Fahd" &&
            i.JobTitle == JobTitle.BackendDeveloper &&
            i.Rating == 4m &&
            i.Description == InstructorDescription);
        for (var index = jasonCount; index < SampleJasonFahdCopies; index++)
        {
            var instructor = new Instructor
            {
                Name = "Jason Fahd",
                JobTitle = JobTitle.BackendDeveloper,
                Rating = 4m,
                Description = InstructorDescription,
                ImagePath = string.Empty
            };
            db.Instructors.Add(instructor);
            instructors.Add(instructor);
        }

        // Course mockups consistently name Ronald Richards as instructor.
        // The details mock gives his title/rating but has no image asset.
        if (!instructors.Any(i => i.Name == "Ronald Richards"))
        {
            var ronald = new Instructor
            {
                Name = "Ronald Richards",
                JobTitle = JobTitle.UxUiDesigner,
                Rating = 4.9m,
                Description = InstructorDescription,
                ImagePath = string.Empty
            };
            db.Instructors.Add(ronald);
            instructors.Add(ronald);
        }

        await db.SaveChangesAsync(ct);
        return instructors;
    }

    private static async Task SeedCoursesAsync(AppDbContext db, Category category, Instructor instructor, CancellationToken ct)
    {
        var courses = await db.Courses.Include(c => c.Contents).ToListAsync(ct);

        // Mock IDs were array indexes, not database identifiers. Use the
        // stable natural-key group and reconcile its multiplicity instead.
        var matchingCount = courses.Count(c =>
            c.Name == CourseName &&
            c.CategoryId == category.Id &&
            c.InstructorId == instructor.Id &&
            c.Level == Level.Beginner &&
            c.Cost == 45m &&
            c.TotalHours == 22 &&
            c.Description == CourseDescription &&
            c.Certification == null);

        for (var index = matchingCount; index < SampleCourseCopies; index++)
        {
            var course = new Course
            {
                Name = CourseName,
                CategoryId = category.Id,
                InstructorId = instructor.Id,
                Level = Level.Beginner,
                Cost = 45m,
                TotalHours = 22,
                Description = CourseDescription,
                Certification = null,
                // Frontend artwork was a CSS gradient, not a recoverable image file.
                ImagePath = string.Empty,
                CreatedAt = DateTime.UtcNow,
                Contents = CourseSections.Select((section, sortOrder) => new CourseContent
                {
                    Name = section.Name,
                    LecturesNumber = section.Lectures,
                    Time = section.Minutes,
                    SortOrder = sortOrder
                }).ToList()
            };
            db.Courses.Add(course);
            courses.Add(course);
        }

        await db.SaveChangesAsync(ct);
    }
}
