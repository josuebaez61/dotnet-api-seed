using System;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Services
{
  public class UserTimezoneService : IUserTimezoneService
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserTimezoneService> _logger;
    private const string TIMEZONE_HEADER = "X-Timezone";
    private const string DEFAULT_TIMEZONE = "UTC";

    // Lista de zonas horarias comunes para validación
    private static readonly HashSet<string> ValidTimezones = new()
        {
            "UTC",
            "America/Argentina/Buenos_Aires",
            "America/New_York",
            "America/Chicago",
            "America/Denver",
            "America/Los_Angeles",
            "Europe/London",
            "Europe/Paris",
            "Europe/Madrid",
            "Asia/Tokyo",
            "Asia/Shanghai",
            "Australia/Sydney"
        };

    public UserTimezoneService(IHttpContextAccessor httpContextAccessor, ILogger<UserTimezoneService> logger)
    {
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
    }

    public DateTime ConvertFromUtc(DateTime utcDateTime, string userTimezone)
    {
      try
      {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
          _logger.LogWarning("DateTime {DateTime} is not in UTC format, treating as UTC", utcDateTime);
        }

        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userTimezone);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
      }
      catch (TimeZoneNotFoundException ex)
      {
        _logger.LogError(ex, "Timezone {Timezone} not found, using UTC", userTimezone);
        return utcDateTime; // Retornar la fecha original si no se encuentra la zona horaria
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error converting from UTC to {Timezone}", userTimezone);
        return utcDateTime;
      }
    }

    public DateTime ConvertToUtc(DateTime userDateTime, string userTimezone)
    {
      try
      {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userTimezone);
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(userDateTime, timeZoneInfo);
        return DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
      }
      catch (TimeZoneNotFoundException ex)
      {
        _logger.LogError(ex, "Timezone {Timezone} not found, treating as UTC", userTimezone);
        return DateTime.SpecifyKind(userDateTime, DateTimeKind.Utc);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error converting {DateTime} from {Timezone} to UTC", userDateTime, userTimezone);
        return DateTime.SpecifyKind(userDateTime, DateTimeKind.Utc);
      }
    }

    public DateTime? ConvertFromUtc(DateTime? utcDateTime, string userTimezone)
    {
      if (!utcDateTime.HasValue)
        return null;

      return ConvertFromUtc(utcDateTime.Value, userTimezone);
    }

    public DateTime? ConvertToUtc(DateTime? userDateTime, string userTimezone)
    {
      if (!userDateTime.HasValue)
        return null;

      return ConvertToUtc(userDateTime.Value, userTimezone);
    }

    public string GetCurrentUserTimezone()
    {
      try
      {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Items?.ContainsKey("UserTimezone") == true)
        {
          var timezone = httpContext.Items["UserTimezone"]?.ToString();
          if (!string.IsNullOrEmpty(timezone) && IsValidTimezone(timezone))
          {
            return timezone;
          }
        }

        // Fallback: intentar obtener directamente del header
        if (httpContext?.Request?.Headers?.ContainsKey(TIMEZONE_HEADER) == true)
        {
          var timezone = httpContext.Request.Headers[TIMEZONE_HEADER].ToString();
          if (IsValidTimezone(timezone))
          {
            return timezone;
          }
          else
          {
            _logger.LogWarning("Invalid timezone {Timezone} in header, using default", timezone);
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting user timezone from context");
      }

      return DEFAULT_TIMEZONE;
    }

    public bool IsValidTimezone(string timezoneId)
    {
      if (string.IsNullOrWhiteSpace(timezoneId))
        return false;

      try
      {
        // Primero verificar en nuestra lista de zonas horarias conocidas
        if (ValidTimezones.Contains(timezoneId))
          return true;

        // Luego intentar crear el TimeZoneInfo para validar
        TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        return true;
      }
      catch (TimeZoneNotFoundException)
      {
        return false;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error validating timezone {Timezone}", timezoneId);
        return false;
      }
    }

    public string GetDefaultTimezone()
    {
      return DEFAULT_TIMEZONE;
    }
  }
}
