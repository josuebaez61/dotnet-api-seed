using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.RefreshToken
{
  public class RefreshTokenCommand : IRequest<AuthDataDto>
  {
    public string RefreshToken { get; set; } = string.Empty;
  }
}
