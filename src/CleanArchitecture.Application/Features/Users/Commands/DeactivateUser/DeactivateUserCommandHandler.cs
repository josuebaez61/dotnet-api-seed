using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Commands.DeactivateUser
{
  public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, bool>
  {
    private readonly IApplicationDbContext _context;

    public DeactivateUserCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Check if user is already inactive
      if (!user.IsActive)
      {
        // User is already inactive, return true
        return true;
      }

      // Deactivate the user
      user.IsActive = false;
      user.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync(cancellationToken);

      return true;
    }
  }
}
