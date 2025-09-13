using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.ResetPassword
{
  public class ResetPasswordCommand : IRequest<ApiResponse>
  {
    public ResetPasswordDto Request { get; set; } = new();
  }
}
