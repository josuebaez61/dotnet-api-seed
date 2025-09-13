using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Test.Commands.SendTestEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize] // Proteger todo el controlador con autenticación
  public class TestController : ControllerBase
  {
    private readonly IMediator _mediator;

    public TestController(IMediator mediator)
    {
      _mediator = mediator;
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
      var result = await _mediator.Send(command);
      return Ok(result);
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
  }
}
