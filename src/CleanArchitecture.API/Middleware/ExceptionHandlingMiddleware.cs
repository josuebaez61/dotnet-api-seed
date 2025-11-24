using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.API.Middleware
{
  public class ExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      _logger.LogError(exception, "An unexpected error occurred");

      var response = context.Response;
      response.ContentType = "application/json";

      var apiResponse = new ApiResponse();

      if (exception is ValidationException validationEx)
      {
        apiResponse = ApiResponse.ErrorResponse(
            GetLocalizedMessage(context, "VALIDATION_FAILED"),
            validationEx.Errors,
            errorCode: "VALIDATION_ERROR"
        );
        response.StatusCode = (int)HttpStatusCode.BadRequest;
      }
      else if (exception is Application.Common.Exceptions.ApplicationException appEx)
      {
        apiResponse = ApiResponse.ErrorResponse(
            GetLocalizedMessage(context, appEx.ErrorCode, appEx.Parameters),
            errorCode: appEx.ErrorCode.Replace("ERROR_", "") // Remove ERROR_ prefix from errorCode for API response
        );
        response.StatusCode = GetStatusCodeForApplicationException(appEx);
      }
      else if (exception is UnauthorizedAccessException)
      {
        apiResponse = ApiResponse.ErrorResponse(
            GetLocalizedMessage(context, "Unauthorized"),
            errorCode: "UNAUTHORIZED"
        );
        response.StatusCode = (int)HttpStatusCode.Unauthorized;
      }
      else if (exception is ArgumentException || exception is ArgumentNullException)
      {
        apiResponse = ApiResponse.ErrorResponse(
            GetLocalizedMessage(context, "VALIDATION_FAILED"),
            errorCode: "VALIDATION_ERROR"
        );
        response.StatusCode = (int)HttpStatusCode.BadRequest;
      }
      else
      {
        apiResponse = ApiResponse.ErrorResponse(
            GetLocalizedMessage(context, "INTERNAL_SERVER_ERROR"),
            errorCode: "INTERNAL_SERVER_ERROR"
        );
        response.StatusCode = (int)HttpStatusCode.InternalServerError;
      }

      var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });

      await response.WriteAsync(jsonResponse);
    }

    private static int GetStatusCodeForApplicationException(CleanArchitecture.Application.Common.Exceptions.ApplicationException exception)
    {
      return exception switch
      {
        UserNotFoundError => (int)HttpStatusCode.NotFound,
        UserNotFoundByIdError => (int)HttpStatusCode.NotFound,
        InvalidCredentialsError => (int)HttpStatusCode.Unauthorized,
        AccountDeactivatedError => (int)HttpStatusCode.Forbidden,
        UserAlreadyExistsError => (int)HttpStatusCode.Conflict,
        InvalidPasswordError => (int)HttpStatusCode.BadRequest,
        InvalidRefreshTokenError => (int)HttpStatusCode.Unauthorized,
        PasswordResetCodeInvalidError => (int)HttpStatusCode.BadRequest,
        PasswordResetCodeExpiredError => (int)HttpStatusCode.BadRequest,
        PasswordResetCodeAlreadyUsedError => (int)HttpStatusCode.BadRequest,
        CurrentPasswordIncorrectError => (int)HttpStatusCode.BadRequest,
        PasswordChangeFailedError => (int)HttpStatusCode.BadRequest,
        InsufficientPermissionsError => (int)HttpStatusCode.Forbidden,
        RoleNotFoundError => (int)HttpStatusCode.NotFound,
        RoleNotFoundByIdError => (int)HttpStatusCode.NotFound,
        PermissionNotFoundError => (int)HttpStatusCode.NotFound,
        PermissionNotFoundByIdError => (int)HttpStatusCode.NotFound,
        ValidationError => (int)HttpStatusCode.BadRequest,
        _ => (int)HttpStatusCode.BadRequest
      };
    }

    private static string GetLocalizedMessage(HttpContext context, string key, object? parameters = null)
    {
      try
      {
        var localizationService = context.RequestServices.GetService<ILocalizationService>();
        if (localizationService != null)
        {
          // Create a new LocalizationService with the current culture
          var culture = GetCurrentCulture(context);
          var localizedService = new CleanArchitecture.Application.Common.Services.LocalizationService(culture);

          // Use GetErrorMessage which will add ERROR_ prefix and look up the translation
          var localizedMessage = localizedService.GetErrorMessage(key);

          // Log for debugging
          var logger = context.RequestServices.GetService<ILogger<ExceptionHandlingMiddleware>>();
          logger?.LogInformation("Localized message for key '{Key}' in culture '{Culture}': '{Message}'", key, culture, localizedMessage);

          return localizedMessage;
        }
        else
        {
          var logger = context.RequestServices.GetService<ILogger<ExceptionHandlingMiddleware>>();
          logger?.LogWarning("ILocalizationService not found in DI container");
        }
      }
      catch (Exception ex)
      {
        // Log the exception for debugging
        var logger = context.RequestServices.GetService<ILogger<ExceptionHandlingMiddleware>>();
        logger?.LogError(ex, "Error getting localized message for key '{Key}'", key);
      }

      return key; // Fallback to the key itself
    }

    private static string GetCurrentCulture(HttpContext context)
    {
      // Check query parameter first (higher priority)
      if (context?.Request?.Query?.ContainsKey("culture") == true)
      {
        var culture = context.Request.Query["culture"].ToString().ToLower();
        if (culture == "es" || culture.StartsWith("es-"))
        {
          return "es";
        }
        if (culture == "en" || culture.StartsWith("en-"))
        {
          return "en";
        }
      }

      // Check Accept-Language header
      if (context?.Request?.Headers?.ContainsKey("Accept-Language") == true)
      {
        var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
        if (acceptLanguage.Contains("es"))
        {
          return "es";
        }
      }

      return "en"; // Default to English
    }
  }
}
