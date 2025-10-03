using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Commands.UpdateRolePermissions
{
  public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, Unit>
  {
    private readonly IApplicationDbContext _context;

    public UpdateRolePermissionsCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Unit> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
      // Verificar que el rol existe
      var role = await _context.Roles
          .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

      if (role == null)
      {
        throw new RoleNotFoundError();
      }

      // Verificar que todos los permisos existen
      var existingPermissionIds = await _context.Permissions
          .Where(p => request.Request.PermissionIds.Contains(p.Id))
          .Select(p => p.Id)
          .ToListAsync(cancellationToken);

      var invalidPermissionIds = request.Request.PermissionIds
          .Except(existingPermissionIds)
          .ToList();

      if (invalidPermissionIds.Any())
      {
        throw new PermissionNotFoundError();
      }

      // Eliminar TODAS las relaciones existentes para este rol
      var existingRolePermissions = await _context.RolePermissions
          .Where(rp => rp.RoleId == request.RoleId)
          .ToListAsync(cancellationToken);

      _context.RolePermissions.RemoveRange(existingRolePermissions);

      // Crear NUEVAS relaciones solo con los permisos del payload
      var newRolePermissions = request.Request.PermissionIds.Select(permissionId => new RolePermission
      {
        Id = Guid.NewGuid(),
        RoleId = request.RoleId,
        PermissionId = permissionId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        IsDeleted = false
      }).ToList();

      _context.RolePermissions.AddRange(newRolePermissions);

      // Actualizar el timestamp del rol
      role.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
