using System.CommandLine;
using System.Text;
using CleanArchitecture.API.Commands;
using CleanArchitecture.API.Extensions;
using CleanArchitecture.API.Middleware;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Check if we're running CLI commands
if (args.Length > 0 && IsCliCommand(args[0]))
{
    await RunCliCommands(args);
    return;
}

// Continue with normal API startup

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Add global route prefix
    options.UseGeneralRoutePrefix("api/v1");
});
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clean Architecture API", Version = "v1" });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add custom services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "es" };
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});


// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var issuer = jwtSettings["Issuer"] ?? "CleanArchitecture";
var audience = jwtSettings["Audience"] ?? "CleanArchitectureUsers";

builder.Services.AddAuthentication(options =>
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
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // New Permission System Policies
    options.AddPolicy("manage.roles", policy => policy.RequireClaim("permission", "manage.roles"));
    options.AddPolicy("manage.users", policy => policy.RequireClaim("permission", "manage.users"));
    options.AddPolicy("manage.user.roles", policy => policy.RequireClaim("permission", "manage.user.roles"));
    options.AddPolicy("manage.role.permissions", policy => policy.RequireClaim("permission", "manage.role.permissions"));
    options.AddPolicy("admin", policy => policy.RequireClaim("permission", "admin"));
    options.AddPolicy("superAdmin", policy => policy.RequireClaim("permission", "superAdmin"));

    // Multiple Permission Policies (ANY of the permissions)
    options.AddPolicy("manage.users.or.admin", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "manage.users") ||
            context.User.HasClaim("permission", "admin") ||
            context.User.HasClaim("permission", "superAdmin")));

    options.AddPolicy("manage.roles.or.admin", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "manage.roles") ||
            context.User.HasClaim("permission", "admin") ||
            context.User.HasClaim("permission", "superAdmin")));

    options.AddPolicy("admin.or.superadmin", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "admin") ||
            context.User.HasClaim("permission", "superAdmin")));

    // Multiple Permission Policies (ALL of the permissions)
    options.AddPolicy("manage.users.and.roles", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "manage.users") &&
            context.User.HasClaim("permission", "manage.roles")));

    // Legacy Policies (Deprecated but still supported)
    options.AddPolicy("Users.Read", policy => policy.RequireClaim("permission", "Users.Read"));
    options.AddPolicy("Users.Write", policy => policy.RequireClaim("permission", "Users.Write"));
    options.AddPolicy("Users.Delete", policy => policy.RequireClaim("permission", "Users.Delete"));
    options.AddPolicy("Roles.Read", policy => policy.RequireClaim("permission", "Roles.Read"));
    options.AddPolicy("Roles.Write", policy => policy.RequireClaim("permission", "Roles.Write"));
    options.AddPolicy("Permissions.Read", policy => policy.RequireClaim("permission", "Permissions.Read"));
    options.AddPolicy("Permissions.Write", policy => policy.RequireClaim("permission", "Permissions.Write"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",    // React default
                "http://localhost:3001",    // React alternative
                "http://localhost:4200",    // Angular default
                "http://localhost:5173",    // Vite default
                "http://localhost:8080",    // Vue default
                "https://localhost:3000",   // HTTPS variants
                "https://localhost:3001",
                "https://localhost:4200",
                "https://localhost:5173",
                "https://localhost:8080"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Permite cookies y headers de auth
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Configure localization
var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

// Add user timezone middleware (must be before exception handling)
app.UseMiddleware<UserTimezoneMiddleware>();

// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Initialize database and create admin user
using (var scope = app.Services.CreateScope())
{
    var initializationService = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    await initializationService.InitializeAsync();
}

app.Run();

// Helper functions for CLI commands
static bool IsCliCommand(string arg)
{
    var cliCommands = new[] { "seed", "truncate", "all", "run", "list" };
    return cliCommands.Contains(arg, StringComparer.OrdinalIgnoreCase);
}

static async Task RunCliCommands(string[] args)
{
    try
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Build host for CLI
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add infrastructure services
                services.AddInfrastructure(configuration);

                // Add CLI commands
                services.AddScoped<SeederCommand>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        // Create the seeder command
        using var scope = host.Services.CreateScope();
        var seederCommand = scope.ServiceProvider.GetRequiredService<SeederCommand>();
        var rootCommand = seederCommand.CreateCommand();

        // Execute the command
        await rootCommand.InvokeAsync(args);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
        Environment.Exit(1);
    }
}
