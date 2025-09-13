using System;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.ChangePassword
{
  public class ChangePasswordCommand : IRequest<bool>
  {
    public Guid UserId { get; set; }
    public ChangePasswordRequestDto Request { get; set; } = new();
  }
}
