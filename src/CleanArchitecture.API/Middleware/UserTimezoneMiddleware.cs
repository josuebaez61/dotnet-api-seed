using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.API.Middleware
{
  public class UserTimezoneMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<UserTimezoneMiddleware> _logger;
    private const string TIMEZONE_HEADER = "X-Timezone";
    private const string DEFAULT_TIMEZONE = "UTC";

    public UserTimezoneMiddleware(RequestDelegate next, ILogger<UserTimezoneMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        // Extraer la zona horaria del header
        var timezone = context.Request.Headers[TIMEZONE_HEADER].FirstOrDefault() ?? DEFAULT_TIMEZONE;

        // Validar la zona horaria
        if (!IsValidTimezone(timezone))
        {
          _logger.LogWarning("Invalid timezone {Timezone} in header, using default UTC", timezone);
          timezone = DEFAULT_TIMEZONE;
        }

        // Agregar la zona horaria al contexto para uso posterior
        context.Items["UserTimezone"] = timezone;

        _logger.LogDebug("User timezone set to: {Timezone}", timezone);
      }
      catch (System.Exception ex)
      {
        _logger.LogError(ex, "Error processing timezone header");
        context.Items["UserTimezone"] = DEFAULT_TIMEZONE;
      }

      await _next(context);
    }

    private bool IsValidTimezone(string timezoneId)
    {
      if (string.IsNullOrWhiteSpace(timezoneId))
        return false;

      try
      {
        TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        return true;
      }
      catch (TimeZoneNotFoundException)
      {
        return false;
      }
      catch
      {
        return false;
      }
    }
  }
}
