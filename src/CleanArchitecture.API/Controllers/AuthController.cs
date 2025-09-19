using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Auth.Commands.ChangeFirstTimePassword;
using CleanArchitecture.Application.Features.Auth.Commands.ChangePassword;
using CleanArchitecture.Application.Features.Auth.Commands.Login;
using CleanArchitecture.Application.Features.Auth.Commands.RefreshToken;
using CleanArchitecture.Application.Features.Auth.Commands.Register;
using CleanArchitecture.Application.Features.Auth.Commands.RequestEmailChange;
using CleanArchitecture.Application.Features.Auth.Commands.RequestPasswordReset;
using CleanArchitecture.Application.Features.Auth.Commands.ResetPassword;
using CleanArchitecture.Application.Features.Auth.Commands.VerifyEmailChange;
using CleanArchitecture.Application.Features.Auth.Queries.GetAuthUser;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly ILocalizationService _localizationService;
    public AuthController(IMediator mediator, ILocalizationService localizationService)
    {
      _localizationService = localizationService;
      _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthDataDto>>> Login([FromBody] LoginRequestDto request)
    {
      var command = new LoginCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<AuthDataDto>.SuccessResponse(result));
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthDataDto>>> Register([FromBody] RegisterRequestDto request)
    {
      var command = new RegisterCommand { Request = request };
      var result = await _mediator.Send(command);
      return CreatedAtAction(nameof(Login), ApiResponse<AuthDataDto>.SuccessResponse(result));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthDataDto>>> RefreshToken([FromHeader(Name = "X-Refresh-Token")] string? refreshToken)
    {
      if (string.IsNullOrEmpty(refreshToken))
      {
        return BadRequest(ApiResponse<AuthDataDto>.ErrorResponse("Refresh token is required in X-Refresh-Token header"));
      }

      var command = new RefreshTokenCommand { RefreshToken = refreshToken };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<AuthDataDto>.SuccessResponse(result));
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
    public async Task<ActionResult<ApiResponse<AuthUserDto>>> GetCurrentUser()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized(ApiResponse<AuthUserDto>.ErrorResponse("Invalid user token"));
      }

      var query = new GetAuthUserQuery { Id = userId };
      var result = await _mediator.Send(query);

      if (result == null)
      {
        return NotFound(ApiResponse<AuthUserDto>.ErrorResponse("User not found"));
      }

      return Ok(ApiResponse<AuthUserDto>.SuccessResponse(result));
    }

    [HttpPost("request-password-reset")]
    public async Task<ActionResult<ApiResponse<PasswordResetResponseDto>>> RequestPasswordReset([FromBody] RequestPasswordResetDto request)
    {
      var command = new RequestPasswordResetCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<PasswordResetResponseDto>.SuccessResponse(result, _localizationService.GetSuccessMessage("PASSWORD_RESET_CODE_SENT", [])));
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordDto request)
    {
      var command = new ResetPasswordCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(result);
    }

    [HttpPost("request-email-change")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RequestEmailChange([FromBody] RequestEmailChangeDto request)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
      {
        return Unauthorized(ApiResponse.ErrorResponse("Invalid user token"));
      }

      var command = new RequestEmailChangeCommand
      {
        UserId = userId,
        Request = request
      };

      var result = await _mediator.Send(command);
      return Ok(result);
    }

    [HttpPost("verify-email-change")]
    public async Task<ActionResult<ApiResponse>> VerifyEmailChange([FromBody] VerifyEmailChangeDto request)
    {
      var command = new VerifyEmailChangeCommand { Request = request };
      var result = await _mediator.Send(command);
      return Ok(result);
    }

    [HttpPost("change-first-time-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AuthDataDto>>> ChangeFirstTimePassword(
        [FromBody] FirstTimePasswordChangeRequestDto request)
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
      {
        return Unauthorized(ApiResponse.ErrorResponse("Invalid user token"));
      }

      var command = new ChangeFirstTimePasswordCommand
      {
        UserId = userIdGuid,
        Request = request
      };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<AuthDataDto>.SuccessResponse(result, "Password changed successfully"));
    }
  }
}
