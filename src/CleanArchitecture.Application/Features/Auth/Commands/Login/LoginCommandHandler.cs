using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.Login
{
  public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
  {
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
      _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
      return await _authService.LoginAsync(request.Request);
    }
  }
}
