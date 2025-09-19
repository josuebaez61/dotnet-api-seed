using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface IAuthService
  {
    Task<AuthDataDto> LoginAsync(LoginRequestDto request);
    Task<AuthDataDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthDataDto> RefreshTokenAsync(string refreshToken);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
    Task<bool> LogoutAsync(string refreshToken);
    Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername);
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    Task<AuthDataDto> GenerateAuthDataAsync(User user);
    Task<string> GeneratePasswordResetCodeAsync(Guid userId);
    Task<bool> ValidatePasswordResetCodeAsync(Guid userId, string code);
    Task<Guid> ValidatePasswordResetCodeAndGetUserIdAsync(string code);
    Task MarkPasswordResetCodeAsUsedAsync(Guid userId, string code);
  }
}
