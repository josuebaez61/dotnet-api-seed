using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Commands.AssignUsersToRole
{
  /// <summary>
  /// Handler for assigning multiple users to a role
  /// </summary>
  public class AssignUsersToRoleCommandHandler : IRequestHandler<AssignUsersToRoleCommand, List<UserDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AssignUsersToRoleCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<UserDto>> Handle(AssignUsersToRoleCommand request, CancellationToken cancellationToken)
    {
      // Verificar que el rol existe
      var role = await _context.Roles
          .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Verificar que todos los usuarios existen
      var existingUsers = await _context.Users
          .Where(u => request.UserIds.Contains(u.Id))
          .ToListAsync(cancellationToken);

      if (existingUsers.Count != request.UserIds.Count)
      {
        var foundUserIds = existingUsers.Select(u => u.Id).ToList();
        var missingUserIds = request.UserIds.Except(foundUserIds).ToList();
        throw new ArgumentException($"Usuarios no encontrados: {string.Join(", ", missingUserIds)}");
      }

      // Obtener usuarios que ya tienen el rol asignado
      var existingUserRoles = await _context.UserRoles
          .Where(ur => ur.RoleId == request.RoleId && request.UserIds.Contains(ur.UserId))
          .ToListAsync(cancellationToken);

      var alreadyAssignedUserIds = existingUserRoles.Select(ur => ur.UserId).ToList();

      // Filtrar usuarios que no tienen el rol asignado
      var usersToAssign = request.UserIds.Except(alreadyAssignedUserIds).ToList();

      if (usersToAssign.Any())
      {
        // Crear nuevas asignaciones de usuario-rol
        var newUserRoles = usersToAssign.Select(userId => new UserRole
        {
          UserId = userId,
          RoleId = request.RoleId
        }).ToList();

        await _context.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
      }

      // Retornar todos los usuarios asignados (tanto los nuevos como los que ya tenían el rol)
      var allAssignedUsers = await _context.Users
          .Where(u => request.UserIds.Contains(u.Id))
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .ToListAsync(cancellationToken);

      return _mapper.Map<List<UserDto>>(allAssignedUsers);
    }
  }
}
