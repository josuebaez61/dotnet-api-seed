using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.Login
{
  public class LoginCommand : IRequest<AuthResponseDto>
  {
    public LoginRequestDto Request { get; set; } = new();
  }
}
