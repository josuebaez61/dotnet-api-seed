using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("admin")]
  [Authorize]
  public class AdminController : ControllerBase
  {
    private readonly ICleanupService _cleanupService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ICleanupService cleanupService, ILogger<AdminController> logger)
    {
      _cleanupService = cleanupService;
      _logger = logger;
    }

    /// <summary>
    /// Endpoint de prueba para verificar que el controlador funciona
    /// </summary>
    /// <returns>Mensaje de prueba</returns>
    [HttpGet("test")]
    public ActionResult<ApiResponse> Test()
    {
      return Ok(ApiResponse.SuccessResponse("Admin controller is working"));
    }

    /// <summary>
    /// Ejecuta limpieza manual de códigos expirados
    /// </summary>
    /// <returns>Resultado de la limpieza</returns>
    [HttpPost("cleanup-expired-codes")]
    public async Task<ActionResult<ApiResponse>> CleanupExpiredCodes()
    {
      try
      {
        _logger.LogInformation("Manual cleanup of expired codes requested by user {UserId}", User.Identity?.Name);

        await _cleanupService.CleanupExpiredCodesAsync();

        return Ok(ApiResponse.SuccessResponse("Expired codes cleaned up successfully"));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during manual cleanup of expired codes");
        return StatusCode(500, ApiResponse.ErrorResponse("An error occurred during cleanup"));
      }
    }
  }
}
