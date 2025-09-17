using CleanArchitecture.API.Helpers;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Examples
{
  /// <summary>
  /// Controlador de ejemplo para mostrar cómo usar el sistema de zona horaria
  /// </summary>
  [ApiController]
  [Route("api/v1/[controller]")]
  public class UserTimezoneExampleController : ControllerBase
  {
    private readonly IUserTimezoneService _timezoneService;

    public UserTimezoneExampleController(IUserTimezoneService timezoneService)
    {
      _timezoneService = timezoneService;
    }

    /// <summary>
    /// Ejemplo de cómo obtener la zona horaria actual del usuario
    /// </summary>
    [HttpGet("current-timezone")]
    public IActionResult GetCurrentTimezone()
    {
      var currentTimezone = _timezoneService.GetCurrentUserTimezone();
      return Ok(new { Timezone = currentTimezone });
    }

    /// <summary>
    /// Ejemplo de cómo convertir una fecha del cliente a UTC
    /// </summary>
    [HttpPost("convert-to-utc")]
    public IActionResult ConvertToUtc([FromBody] DateTime clientDateTime)
    {
      // Convertir la fecha del cliente (en su zona horaria) a UTC
      var utcDateTime = UserTimezoneHelper.ConvertClientDateTimeToUtc(clientDateTime, _timezoneService, HttpContext);

      return Ok(new
      {
        OriginalDateTime = clientDateTime,
        UtcDateTime = utcDateTime,
        UserTimezone = _timezoneService.GetCurrentUserTimezone()
      });
    }

    /// <summary>
    /// Ejemplo de cómo convertir una fecha UTC a la zona horaria del usuario
    /// </summary>
    [HttpPost("convert-from-utc")]
    public IActionResult ConvertFromUtc([FromBody] DateTime utcDateTime)
    {
      // Convertir la fecha UTC a la zona horaria del usuario
      var userDateTime = UserTimezoneHelper.ConvertUtcToUserTimezone(utcDateTime, _timezoneService, HttpContext);

      return Ok(new
      {
        UtcDateTime = utcDateTime,
        UserDateTime = userDateTime,
        UserTimezone = _timezoneService.GetCurrentUserTimezone()
      });
    }

    /// <summary>
    /// Ejemplo de validación de zona horaria
    /// </summary>
    [HttpGet("validate-timezone/{timezoneId}")]
    public IActionResult ValidateTimezone(string timezoneId)
    {
      var isValid = _timezoneService.IsValidTimezone(timezoneId);

      return Ok(new
      {
        TimezoneId = timezoneId,
        IsValid = isValid
      });
    }

    /// <summary>
    /// Ejemplo de cómo las fechas se manejan automáticamente en Entity Framework
    /// </summary>
    [HttpGet("ef-utc-demo")]
    public IActionResult EfUtcDemo()
    {
      var now = DateTime.Now; // Fecha local del servidor
      var utcNow = DateTime.UtcNow; // Fecha UTC

      return Ok(new
      {
        Message = "Las fechas se convierten automáticamente a UTC en Entity Framework",
        ServerLocalTime = now,
        ServerUtcTime = utcNow,
        UserTimezone = _timezoneService.GetCurrentUserTimezone(),
        Note = "Cuando guardes cualquier entidad con DateTime, EF Core automáticamente convertirá a UTC"
      });
    }
  }
}
