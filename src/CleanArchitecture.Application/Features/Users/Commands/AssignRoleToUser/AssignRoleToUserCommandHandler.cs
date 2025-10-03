using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Commands.AssignRoleToUser
{
  public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, RoleDto>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AssignRoleToUserCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<RoleDto> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
      // Verificar que el usuario existe
      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Verificar que el rol existe
      var role = await _context.Roles
          .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Verificar si el usuario ya tiene asignado este rol
      var existingUserRole = await _context.UserRoles
          .FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, cancellationToken);

      if (existingUserRole != null)
      {
        throw new ArgumentException("User already has this role assigned");
      }

      // Asignar el rol al usuario
      var userRole = new UserRole
      {
        UserId = request.UserId,
        RoleId = request.RoleId
      };

      await _context.UserRoles.AddAsync(userRole, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);

      return _mapper.Map<RoleDto>(role);
    }
  }
}
