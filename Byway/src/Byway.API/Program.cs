using Byway.API.Middleware;
using Byway.Application;
using Byway.Infrastructure;
using Byway.Infrastructure.Seed;

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
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
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

// ---- Seed (roles, admin account, categories) ----------------------------
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
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
app.UseAuthorization();

app.MapControllers();

app.Run();
