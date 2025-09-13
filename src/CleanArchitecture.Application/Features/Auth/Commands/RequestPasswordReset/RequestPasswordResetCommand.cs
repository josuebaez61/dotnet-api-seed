using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.RequestPasswordReset
{
  public class RequestPasswordResetCommand : IRequest<PasswordResetResponseDto>
  {
    public RequestPasswordResetDto Request { get; set; } = new();
  }
}
