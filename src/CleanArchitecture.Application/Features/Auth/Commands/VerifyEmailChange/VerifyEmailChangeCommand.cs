using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.VerifyEmailChange
{
  public class VerifyEmailChangeCommand : IRequest<ApiResponse>
  {
    public VerifyEmailChangeDto Request { get; set; } = new VerifyEmailChangeDto();
  }
}
