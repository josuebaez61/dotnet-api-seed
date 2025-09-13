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
  public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, ApiResponse>
  {
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localizationService;

    public UpdateRolePermissionsCommandHandler(
        IApplicationDbContext context,
        ILocalizationService localizationService)
    {
      _context = context;
      _localizationService = localizationService;
    }

    public async Task<ApiResponse> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
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

      // Obtener los permisos actuales del rol
      var currentRolePermissions = await _context.RolePermissions
          .Where(rp => rp.RoleId == request.RoleId)
          .ToListAsync(cancellationToken);

      // Eliminar los permisos que ya no están en la nueva lista
      var permissionsToRemove = currentRolePermissions
          .Where(rp => !request.Request.PermissionIds.Contains(rp.PermissionId))
          .ToList();

      foreach (var permissionToRemove in permissionsToRemove)
      {
        permissionToRemove.IsDeleted = true;
        permissionToRemove.UpdatedAt = DateTime.UtcNow;
      }

      // Agregar los nuevos permisos que no estaban asignados
      var currentPermissionIds = currentRolePermissions
          .Where(rp => !rp.IsDeleted)
          .Select(rp => rp.PermissionId)
          .ToList();

      var permissionsToAdd = request.Request.PermissionIds
          .Except(currentPermissionIds)
          .ToList();

      foreach (var permissionId in permissionsToAdd)
      {
        // Verificar si ya existe un RolePermission para este rol y permiso (aunque esté marcado como eliminado)
        var existingRolePermission = currentRolePermissions
            .FirstOrDefault(rp => rp.PermissionId == permissionId);

        if (existingRolePermission != null)
        {
          // Si existe pero está marcado como eliminado, reactivarlo
          existingRolePermission.IsDeleted = false;
          existingRolePermission.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
          // Si no existe, crear uno nuevo
          var newRolePermission = new RolePermission
          {
            Id = Guid.NewGuid(),
            RoleId = request.RoleId,
            PermissionId = permissionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
          };

          _context.RolePermissions.Add(newRolePermission);
        }
      }

      // Actualizar el timestamp del rol
      role.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync(cancellationToken);

      return ApiResponse.SuccessResponse("Role permissions updated successfully");
    }
  }
}
