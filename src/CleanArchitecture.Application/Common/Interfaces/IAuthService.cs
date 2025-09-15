using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface IAuthService
  {
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
    Task<bool> LogoutAsync(string refreshToken);
    Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername);
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    Task<AuthResponseDto> GenerateAuthResponseAsync(User user);
    Task<string> GeneratePasswordResetCodeAsync(Guid userId);
    Task<bool> ValidatePasswordResetCodeAsync(Guid userId, string code);
    Task MarkPasswordResetCodeAsUsedAsync(Guid userId, string code);
  }
}
