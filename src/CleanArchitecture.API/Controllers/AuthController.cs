using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Auth.Commands.ChangePassword;
using CleanArchitecture.Application.Features.Auth.Commands.Login;
using CleanArchitecture.Application.Features.Auth.Commands.RefreshToken;
using CleanArchitecture.Application.Features.Auth.Commands.Register;
using CleanArchitecture.Application.Features.Auth.Commands.RequestPasswordReset;
using CleanArchitecture.Application.Features.Auth.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
      var command = new LoginCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
      var command = new RegisterCommand { Request = request };
      var result = await _mediator.Send(command);
      return CreatedAtAction(nameof(Login), ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
      var command = new RefreshTokenCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized(ApiResponse.ErrorResponse("Invalid user token"));
      }

      var command = new ChangePasswordCommand
      {
        UserId = userId,
        Request = request
      };

      var result = await _mediator.Send(command);

      if (result)
      {
        return Ok(ApiResponse.SuccessResponse("Password changed successfully"));
      }

      return BadRequest(ApiResponse.ErrorResponse("Failed to change password"));
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponse<object>> GetCurrentUser()
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var userName = User.FindFirst(ClaimTypes.Name)?.Value;
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      var firstName = User.FindFirst("firstName")?.Value;
      var lastName = User.FindFirst("lastName")?.Value;
      var isActive = User.FindFirst("isActive")?.Value;

      var userInfo = new
      {
        Id = userId,
        UserName = userName,
        Email = email,
        FirstName = firstName,
        LastName = lastName,
        IsActive = isActive
      };

      return Ok(ApiResponse<object>.SuccessResponse(userInfo));
    }

    [HttpPost("request-password-reset")]
    public async Task<ActionResult<ApiResponse<PasswordResetResponseDto>>> RequestPasswordReset([FromBody] RequestPasswordResetDto request)
    {
      var command = new RequestPasswordResetCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<PasswordResetResponseDto>.SuccessResponse(result));
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordDto request)
    {
      var command = new ResetPasswordCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(result);
    }
  }
}
