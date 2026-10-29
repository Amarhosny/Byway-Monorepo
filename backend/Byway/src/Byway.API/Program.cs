using Byway.API.Middleware;
using Byway.Application;
using Byway.Infrastructure;
using Byway.Infrastructure.Persistence;
using Byway.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---- Services ----------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Byway API", Version = "v1" });

    // JWT support in the Swagger UI "Authorize" button
    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT access token. Swagger adds the Bearer prefix."
    });
    options.AddSecurityRequirement(new()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Application + Infrastructure — see each layer's DependencyInjection.cs.
// Order matters: Infrastructure registers the real IAuthService and must be
// called (it is not overridden by Application, since Application never
// registers IAuthService itself — see Byway.Application.DependencyInjection).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS — configuration-driven, not hardcoded, so Development and Production
// can allow different origins without a code change.
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (args.Contains("--initialize-database", StringComparer.OrdinalIgnoreCase))
{
    try
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(scope.ServiceProvider);
        Console.WriteLine("Database initialization completed successfully.");
        await app.DisposeAsync();
        return 0;
    }
    catch (Exception)
    {
        Console.Error.WriteLine("Database initialization failed. Check database configuration and availability.");
        await app.DisposeAsync();
        return 1;
    }
}

// ---- Middleware pipeline -------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling(); // must be first so it can catch everything downstream

app.UseHttpsRedirection();
app.UseStaticFiles(); // serves wwwroot/images/* (Course/Instructor images)

app.UseCors("Default");

app.UseAuthentication();
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        await next();

        if (context.Request.Path.StartsWithSegments("/api")
            && context.Response.StatusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden)
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Byway.ApiAuthDiagnostics");
            var authorization = context.Request.Headers.Authorization.ToString();
            var bearerPrefixPresent = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
            logger.LogWarning(
                "API authorization response: method={Method}; path={Path}; status={StatusCode}; authorizationHeaderPresent={HeaderPresent}; bearerPrefixPresent={BearerPrefixPresent}; authenticated={Authenticated}; adminRolePresent={AdminRolePresent}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                !string.IsNullOrWhiteSpace(authorization),
                bearerPrefixPresent,
                context.User.Identity?.IsAuthenticated == true,
                context.User.IsInRole(Byway.Infrastructure.Identity.IdentityRoles.Admin));
        }
    });
}
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
return 0;
