using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.RefreshToken
{
  public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthDataDto>
  {
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
      _authService = authService;
    }

    public async Task<AuthDataDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
      return await _authService.RefreshTokenAsync(request.RefreshToken);
    }
  }
}
