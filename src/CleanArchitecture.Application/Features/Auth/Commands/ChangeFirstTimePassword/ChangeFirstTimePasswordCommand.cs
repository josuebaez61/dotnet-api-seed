using System;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.ChangeFirstTimePassword
{
  public class ChangeFirstTimePasswordCommand : IRequest<AuthDataDto>
  {
    public Guid UserId { get; set; }
    public FirstTimePasswordChangeRequestDto Request { get; set; } = new();
  }
}
