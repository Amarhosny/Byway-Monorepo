using Byway.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Byway.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers every Application-layer use-case service EXCEPT IAuthService.
    /// IAuthService's implementation needs UserManager/SignInManager/RoleManager
    /// from ASP.NET Core Identity, which lives in Infrastructure — Application
    /// must not reference Identity directly. Byway.Infrastructure.DependencyInjection
    /// registers IAuthService itself. Program.cs must call AddApplication()
    /// AND AddInfrastructure() for the API to have a working IAuthService.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IInstructorService, InstructorService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
