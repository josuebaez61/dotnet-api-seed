using CleanArchitecture.API.Common;

namespace CleanArchitecture.API.Middleware
{
  /// <summary>
  /// Middleware to validate environment configuration
  /// </summary>
  public class EnvironmentValidationMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<EnvironmentValidationMiddleware> _logger;

    public EnvironmentValidationMiddleware(RequestDelegate next, ILogger<EnvironmentValidationMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      // Only validate environment on startup (first request)
      if (!context.Items.ContainsKey("EnvironmentValidated"))
      {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        try
        {
          EnvironmentConstants.ValidateEnvironment(environment);
          context.Items["EnvironmentValidated"] = true;

          _logger.LogInformation("✅ Environment validation passed: {Environment}", environment);
        }
        catch (InvalidOperationException ex)
        {
          _logger.LogError("❌ {ErrorMessage}", ex.Message);
          _logger.LogError("🔧 Current environment: '{Environment}'", environment);
          _logger.LogError("💡 Please set ASPNETCORE_ENVIRONMENT to one of: {AllowedEnvironments}",
              EnvironmentConstants.GetAllowedEnvironmentsString());

          // Return error response instead of continuing
          context.Response.StatusCode = 500;
          context.Response.ContentType = "application/json";

          var errorResponse = new
          {
            error = "Environment Configuration Error",
            message = ex.Message,
            currentEnvironment = environment,
            allowedEnvironments = EnvironmentConstants.AllowedEnvironments,
            timestamp = DateTime.UtcNow
          };

          await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
          return;
        }
      }

      await _next(context);
    }
  }

  /// <summary>
  /// Extension methods for EnvironmentValidationMiddleware
  /// </summary>
  public static class EnvironmentValidationMiddlewareExtensions
  {
    /// <summary>
    /// Adds environment validation middleware to the pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseEnvironmentValidation(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<EnvironmentValidationMiddleware>();
    }
  }
}
