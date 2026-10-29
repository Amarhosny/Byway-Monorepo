using Byway.Application.Interfaces.Repositories;
using Byway.Application.Interfaces.Services;
using Byway.Application.Services;
using Byway.Infrastructure.Identity;
using Byway.Infrastructure.Persistence;
using Byway.Infrastructure.Repositories;
using Byway.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Byway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Reasonable defaults for a graduation project — adjust if
                // the grading rubric wants specific password rules.
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        ValidateJwtOptions(jwt);
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (IsDevelopmentDashboardRequest(context.HttpContext))
                    {
                        var authorization = context.Request.Headers.Authorization.ToString();
                        var bearerPrefixPresent = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
                        GetDashboardAuthLogger(context.HttpContext).LogInformation(
                            "Dashboard Authorization header: present={HeaderPresent}; bearerPrefixPresent={BearerPrefixPresent}; tokenPortionPresent={TokenPortionPresent}",
                            !string.IsNullOrWhiteSpace(authorization),
                            bearerPrefixPresent,
                            bearerPrefixPresent && !string.IsNullOrWhiteSpace(authorization["Bearer ".Length..]));
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (IsDevelopmentDashboardRequest(context.HttpContext))
                        GetDashboardAuthLogger(context.HttpContext).LogWarning(
                            "Dashboard JWT validation failed: failureCategory={FailureCategory}",
                            GetJwtFailureCategory(context.Exception));
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    if (IsDevelopmentDashboardRequest(context.HttpContext))
                        GetDashboardAuthLogger(context.HttpContext).LogInformation(
                            "Dashboard JWT validated: adminRolePresent={AdminRolePresent}",
                            context.Principal?.IsInRole(IdentityRoles.Admin) == true);
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    if (IsDevelopmentDashboardRequest(context.HttpContext))
                        GetDashboardAuthLogger(context.HttpContext).LogWarning(
                            "Dashboard authentication challenge: failureCategory={FailureCategory}",
                            context.AuthenticateFailure is null
                                ? "NoValidBearerToken"
                                : GetJwtFailureCategory(context.AuthenticateFailure));
                    return Task.CompletedTask;
                }
            };
        });

        services.AddHttpContextAccessor();

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // Cross-cutting services
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPaymentSimulator, PaymentSimulator>();

        // IAuthService's real implementation — Application layer only
        // defines the interface; this is the one and only registration.
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    private static void ValidateJwtOptions(JwtOptions jwt)
    {
        var key = jwt.SigningKey;
        var placeholderMarkers = new[] { "PLACEHOLDER", "REPLACE", "YOUR_", "CHANGE_ME", "EXAMPLE", "DEVELOPMENT_ONLY" };
        var isPlaceholder = string.IsNullOrWhiteSpace(key)
            || placeholderMarkers.Any(marker => key.Contains(marker, StringComparison.OrdinalIgnoreCase));

        if (isPlaceholder || Encoding.UTF8.GetByteCount(key) < 32)
            throw new InvalidOperationException("Jwt:SigningKey must be a non-placeholder key of at least 32 UTF-8 bytes.");

        if (string.IsNullOrWhiteSpace(jwt.Issuer))
            throw new InvalidOperationException("Jwt:Issuer must be configured.");

        if (string.IsNullOrWhiteSpace(jwt.Audience))
            throw new InvalidOperationException("Jwt:Audience must be configured.");

        if (jwt.ExpiryMinutes is < 1 or > 1440)
            throw new InvalidOperationException("Jwt:ExpiryMinutes must be between 1 and 1440.");
    }

    private static bool IsDevelopmentDashboardRequest(HttpContext context) =>
        context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
        && context.Request.Path.Equals("/api/admin/dashboard", StringComparison.OrdinalIgnoreCase);

    private static ILogger GetDashboardAuthLogger(HttpContext context) =>
        context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Byway.DashboardAuthDiagnostics");

    private static string GetJwtFailureCategory(Exception exception) => exception switch
    {
        SecurityTokenMalformedException => "MalformedToken",
        SecurityTokenExpiredException => "ExpiredToken",
        SecurityTokenNotYetValidException => "TokenNotYetValid",
        SecurityTokenInvalidIssuerException => "IssuerValidationFailed",
        SecurityTokenInvalidAudienceException => "AudienceValidationFailed",
        SecurityTokenSignatureKeyNotFoundException => "SigningKeyNotFound",
        SecurityTokenInvalidSignatureException => "SignatureValidationFailed",
        SecurityTokenInvalidLifetimeException => "LifetimeValidationFailed",
        _ => "OtherTokenValidationFailure"
    };
}
