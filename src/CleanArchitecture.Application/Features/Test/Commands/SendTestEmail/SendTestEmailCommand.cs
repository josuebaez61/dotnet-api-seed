using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Test.Commands.SendTestEmail
{
  public class SendTestEmailCommand : IRequest<ApiResponse>
  {
    public TestEmailRequestDto Request { get; set; } = new();
  }
}
