using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.Register
{
  public class RegisterCommand : IRequest<AuthResponseDto>
  {
    public RegisterRequestDto Request { get; set; } = new();
  }
}
