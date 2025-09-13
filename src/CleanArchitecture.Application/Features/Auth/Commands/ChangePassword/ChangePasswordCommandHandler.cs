using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitecture.Application.Features.Auth.Commands.ChangePassword
{
  public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
  {
    private readonly IAuthService _authService;

    public ChangePasswordCommandHandler(IAuthService authService)
    {
      _authService = authService;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
      return await _authService.ChangePasswordAsync(request.UserId, request.Request);
    }
  }
}
