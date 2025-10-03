using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Test.Commands.SendTestEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize] // Proteger todo el controlador con autenticación
  public class TestController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly ILocalizationService _localizationService;
    private readonly IEmailService _emailService;
    private readonly ILogger<TestController> _logger;

    public TestController(IMediator mediator, ILocalizationService localizationService, IEmailService emailService, ILogger<TestController> logger)
    {
      _mediator = mediator;
      _localizationService = localizationService;
      _emailService = emailService;
      _logger = logger;
    }

    /// <summary>
    /// Envía un correo de prueba
    /// </summary>
    /// <param name="request">Datos del correo de prueba</param>
    /// <returns>Resultado del envío</returns>
    [HttpPost("send-test-email")]
    public async Task<ActionResult<ApiResponse>> SendTestEmail([FromBody] TestEmailRequestDto request)
    {
      var command = new SendTestEmailCommand { Request = request };
      await _mediator.Send(command);
      return Ok(ApiResponse.SuccessResponse(_localizationService.GetSuccessMessage("TEST_EMAIL_SENT")));
    }

    /// <summary>
    /// Endpoint de prueba para verificar autenticación
    /// </summary>
    /// <returns>Información del usuario autenticado</returns>
    [HttpGet("auth-test")]
    public ActionResult<ApiResponse<object>> AuthTest()
    {
      var userInfo = new
      {
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
        UserName = User.Identity?.Name,
        UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
        Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
        Roles = User.Claims
              .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
              .Select(c => c.Value)
              .ToList(),
        Permissions = User.Claims
              .Where(c => c.Type == "permission")
              .Select(c => c.Value)
              .ToList()
      };

      return Ok(ApiResponse<object>.SuccessResponse(userInfo, "Authentication test successful"));
    }

    /// <summary>
    /// Endpoint de prueba para verificar localización
    /// </summary>
    /// <returns>Mensajes localizados de prueba</returns>
    [HttpGet("localization-test")]
    public ActionResult<ApiResponse<object>> LocalizationTest()
    {
      var testMessages = new
      {
        InvalidCredentials = _localizationService.GetErrorMessage("INVALID_CREDENTIALS"),
        UserNotFound = _localizationService.GetErrorMessage("USER_NOT_FOUND"),
        LoginSuccessful = _localizationService.GetSuccessMessage("LOGIN_SUCCESSFUL")
      };

      return Ok(ApiResponse<object>.SuccessResponse(testMessages, "Localization test"));
    }

    /// <summary>
    /// Endpoint de prueba para verificar el envío de emails de password reset
    /// </summary>
    /// <param name="email">Email de destino</param>
    /// <returns>Resultado del envío</returns>
    [HttpPost("test-password-reset-email")]
    public async Task<ActionResult<ApiResponse>> TestPasswordResetEmail([FromBody] string email)
    {
      try
      {
        _logger.LogInformation("Testing password reset email to: {Email}", email);
        
        var testCode = "1234-5678";
        await _emailService.SendPasswordResetEmailAsync(email, "Test User", testCode);
        
        _logger.LogInformation("Password reset test email sent successfully to: {Email}", email);
        
        return Ok(ApiResponse.SuccessResponse($"Password reset test email sent to {email} with code: {testCode}"));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to send password reset test email to: {Email}", email);
        return BadRequest(ApiResponse.ErrorResponse($"Failed to send test email: {ex.Message}"));
      }
    }
  }
}
