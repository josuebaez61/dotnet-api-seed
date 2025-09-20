using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Commands.ActivateUser
{
  public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, bool>
  {
    private readonly IApplicationDbContext _context;

    public ActivateUserCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Check if user is already active
      if (user.IsActive)
      {
        // User is already active, return true
        return true;
      }

      // Activate the user
      user.IsActive = true;
      user.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync(cancellationToken);

      return true;
    }
  }
}
