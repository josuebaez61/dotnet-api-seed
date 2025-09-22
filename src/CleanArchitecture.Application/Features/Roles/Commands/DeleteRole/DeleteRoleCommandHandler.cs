using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Commands.DeleteRole
{
  public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResponse>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;

    public DeleteRoleCommandHandler(
        RoleManager<Role> roleManager,
        IPermissionService permissionService,
        ILocalizationService localizationService)
    {
      _roleManager = roleManager;
      _permissionService = permissionService;
      _localizationService = localizationService;
    }

    public async Task<ApiResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
      // Check if role exists
      var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      var rolePermissions = await _permissionService.GetRolePermissionsAsync(request.RoleId);
      foreach (var permission in rolePermissions)
      {
        await _permissionService.RemovePermissionFromRoleAsync(request.RoleId, permission.Id);
      }

      // Remove all role claims
      var roleClaims = await _roleManager.GetClaimsAsync(role);
      foreach (var claim in roleClaims)
      {
        await _roleManager.RemoveClaimAsync(role, claim);
      }

      // Delete the role
      var result = await _roleManager.DeleteAsync(role);
      if (!result.Succeeded)
      {
        throw new InvalidOperationException(
            $"Failed to delete role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }

      return ApiResponse.SuccessResponse(
          _localizationService.GetSuccessMessage("RoleDeletedSuccessfully"));
    }
  }
}
