using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.RefreshToken
{
  public class RefreshTokenCommand : IRequest<AuthResponseDto>
  {
    public RefreshTokenRequestDto Request { get; set; } = new();
  }
}
