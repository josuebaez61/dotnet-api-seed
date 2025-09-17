using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.API.Helpers
{
  public static class UserTimezoneHelper
  {
    /// <summary>
    /// Convierte una fecha del cliente (en zona horaria del usuario) a UTC
    /// </summary>
    /// <param name="clientDateTime">Fecha enviada por el cliente</param>
    /// <param name="timezoneService">Servicio de zona horaria</param>
    /// <param name="httpContext">Contexto HTTP</param>
    /// <returns>Fecha convertida a UTC</returns>
    public static DateTime ConvertClientDateTimeToUtc(DateTime clientDateTime, IUserTimezoneService timezoneService, HttpContext httpContext)
    {
      // Obtener la zona horaria del usuario desde el contexto
      var userTimezone = httpContext.Items["UserTimezone"]?.ToString() ?? timezoneService.GetDefaultTimezone();

      // Convertir de la zona horaria del usuario a UTC
      return timezoneService.ConvertToUtc(clientDateTime, userTimezone);
    }

    /// <summary>
    /// Convierte una fecha nullable del cliente a UTC
    /// </summary>
    /// <param name="clientDateTime">Fecha nullable enviada por el cliente</param>
    /// <param name="timezoneService">Servicio de zona horaria</param>
    /// <param name="httpContext">Contexto HTTP</param>
    /// <returns>Fecha convertida a UTC o null</returns>
    public static DateTime? ConvertClientDateTimeToUtc(DateTime? clientDateTime, IUserTimezoneService timezoneService, HttpContext httpContext)
    {
      if (!clientDateTime.HasValue)
        return null;

      return ConvertClientDateTimeToUtc(clientDateTime.Value, timezoneService, httpContext);
    }

    /// <summary>
    /// Convierte una fecha UTC a la zona horaria del usuario para respuesta
    /// </summary>
    /// <param name="utcDateTime">Fecha en UTC</param>
    /// <param name="timezoneService">Servicio de zona horaria</param>
    /// <param name="httpContext">Contexto HTTP</param>
    /// <returns>Fecha convertida a la zona horaria del usuario</returns>
    public static DateTime ConvertUtcToUserTimezone(DateTime utcDateTime, IUserTimezoneService timezoneService, HttpContext httpContext)
    {
      var userTimezone = httpContext.Items["UserTimezone"]?.ToString() ?? timezoneService.GetDefaultTimezone();
      return timezoneService.ConvertFromUtc(utcDateTime, userTimezone);
    }

    /// <summary>
    /// Convierte una fecha nullable UTC a la zona horaria del usuario
    /// </summary>
    /// <param name="utcDateTime">Fecha nullable en UTC</param>
    /// <param name="timezoneService">Servicio de zona horaria</param>
    /// <param name="httpContext">Contexto HTTP</param>
    /// <returns>Fecha convertida a la zona horaria del usuario o null</returns>
    public static DateTime? ConvertUtcToUserTimezone(DateTime? utcDateTime, IUserTimezoneService timezoneService, HttpContext httpContext)
    {
      if (!utcDateTime.HasValue)
        return null;

      return ConvertUtcToUserTimezone(utcDateTime.Value, timezoneService, httpContext);
    }
  }
}
