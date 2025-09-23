using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Commands.UnassignUserFromRole
{
  /// <summary>
  /// Handler for unassigning a user from a role
  /// </summary>
  public class UnassignUserFromRoleCommandHandler : IRequestHandler<UnassignUserFromRoleCommand, UserDto>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UnassignUserFromRoleCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<UserDto> Handle(UnassignUserFromRoleCommand request, CancellationToken cancellationToken)
    {
      // Verify that the role exists
      var role = await _context.Roles
          .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Verify that the user exists
      var user = await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Check if the user has the role assigned
      var userRole = await _context.UserRoles
          .FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, cancellationToken);

      if (userRole == null)
      {
        throw new ArgumentException($"User {user.UserName} is not assigned to role {role.Name}");
      }

      // Remove the user-role assignment
      _context.UserRoles.Remove(userRole);
      await _context.SaveChangesAsync(cancellationToken);

      // Return the updated user with their remaining roles
      return _mapper.Map<UserDto>(user);
    }
  }
}
