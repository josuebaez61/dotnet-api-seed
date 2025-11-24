using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.API.Attributes;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Roles.Commands.AssignUsersToRole;
using CleanArchitecture.Application.Features.Roles.Commands.CreateRole;
using CleanArchitecture.Application.Features.Roles.Commands.DeleteRole;
using CleanArchitecture.Application.Features.Roles.Commands.UnassignUserFromRole;
using CleanArchitecture.Application.Features.Roles.Commands.UpdateRole;
using CleanArchitecture.Application.Features.Roles.Commands.UpdateRolePermissions;
using CleanArchitecture.Application.Features.Roles.Queries.GetAllRoles;
using CleanArchitecture.Application.Features.Roles.Queries.GetAssignableRoles;
using CleanArchitecture.Application.Features.Roles.Queries.GetAssignableUsers;
using CleanArchitecture.Application.Features.Roles.Queries.GetRoleById;
using CleanArchitecture.Application.Features.Roles.Queries.GetRolePermissions;
using CleanArchitecture.Application.Features.Roles.Queries.GetRoleUserCount;
using CleanArchitecture.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class RolesController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly ILocalizationService _localizationService;
    public RolesController(IMediator mediator, ILocalizationService localizationService)
    {
      _mediator = mediator;
      _localizationService = localizationService;
    }

    [HttpGet("all")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetAllRoles()
    {
      var query = new GetAllRolesQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result));
    }

    [HttpGet("id/{roleId}")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById([FromRoute] Guid roleId)
    {
      var query = new GetRoleByIdQuery { RoleId = roleId };
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpGet("id/{roleId}/permissions")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetRolePermissions([FromRoute] Guid roleId)
    {
      var query = new GetRolePermissionsQuery { RoleId = roleId };
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(result));
    }

    [HttpPost]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleDto request)
    {
      var command = new CreateRoleCommand { Role = request };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpPut("id/{roleId}/permissions")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse>> UpdateRolePermissions(
        [FromRoute] Guid roleId,
        [FromBody] UpdateRolePermissionsRequestDto request)
    {
      var command = new UpdateRolePermissionsCommand
      {
        RoleId = roleId,
        Request = request
      };
      await _mediator.Send(command);
      return Ok(ApiResponse.SuccessResponse(_localizationService.GetSuccessMessage("ROLE_PERMISSIONS_UPDATED")));
    }

    /// <summary>
    /// Gets the number of users assigned to each role
    /// </summary>
    /// <returns>Dictionary with the structure { [roleId]: number of users }</returns>
    [HttpGet("user-counts")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<RoleUserCountDto>>> GetRoleUserCounts()
    {
      var query = new GetRoleUserCountQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<RoleUserCountDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets a list of roles that can be assigned to a specific user (roles not already assigned to the user)
    /// </summary>
    /// <param name="userId">ID of the user to get assignable roles for</param>
    /// <returns>List of roles that can be assigned to the user</returns>
    [HttpGet("assignable/user/{userId}")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetAssignableRoles([FromRoute] Guid userId)
    {
      var query = new GetAssignableRolesQuery { UserId = userId };
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets a list of users that can be assigned to a specific role (users not already assigned to the role)
    /// </summary>
    /// <param name="roleId">ID of the role to get assignable users for</param>
    /// <returns>List of users that can be assigned to the role</returns>
    [HttpGet("id/{roleId}/assignable-users")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<UserOptionDto>>>> GetAssignableUsers([FromRoute] Guid roleId)
    {
      var query = new GetAssignableUsersQuery { RoleId = roleId };
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<UserOptionDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Actualiza un rol (solo nombre y descripción)
    /// </summary>
    /// <param name="id">ID del rol a actualizar</param>
    /// <param name="request">Datos del rol a actualizar</param>
    /// <returns>Rol actualizado</returns>
    [HttpPatch("id/{id}")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(Guid id, [FromBody] UpdateRoleDto request)
    {
      var command = new UpdateRoleCommand(id, request);
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<RoleDto>.SuccessResponse(result, "Rol actualizado exitosamente"));
    }

    /// <summary>
    /// Assigns multiple users to a specific role
    /// </summary>
    /// <param name="roleId">ID of the role to which users will be assigned</param>
    /// <param name="request">List of user IDs to assign</param>
    /// <returns>List of users assigned to the role</returns>
    [HttpPost("id/{roleId}/assign-users")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> AssignUsersToRole(
        [FromRoute] Guid roleId,
        [FromBody] AssignUsersToRoleRequestDto request)
    {
      var command = new AssignUsersToRoleCommand
      {
        RoleId = roleId,
        UserIds = request.UserIds
      };

      var result = await _mediator.Send(command);
      return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result, "Users assigned to role successfully"));
    }

    /// <summary>
    /// Elimina un rol del sistema
    /// </summary>
    /// <param name="roleId">ID del rol a eliminar</param>
    /// <returns>Respuesta de éxito o error</returns>
    [HttpDelete("id/{roleId}")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse>> DeleteRole([FromRoute] Guid roleId)
    {
      var command = new DeleteRoleCommand { RoleId = roleId };
      await _mediator.Send(command);
      return Ok(ApiResponse.SuccessResponse(_localizationService.GetSuccessMessage("ROLE_DELETED")));
    }

    /// <summary>
    /// Unassigns a user from a specific role
    /// </summary>
    /// <param name="roleId">ID of the role from which the user will be unassigned</param>
    /// <param name="request">User ID to unassign from the role</param>
    /// <returns>Updated user information</returns>
    [HttpPost("id/{roleId}/unassign-user")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UnassignUserFromRole(
        [FromRoute] Guid roleId,
        [FromBody] UnassignUserFromRoleRequestDto request)
    {
      var command = new UnassignUserFromRoleCommand
      {
        RoleId = roleId,
        UserId = request.UserId
      };

      var result = await _mediator.Send(command);
      return Ok(ApiResponse<UserDto>.SuccessResponse(result, "User unassigned from role successfully"));
    }
  }
}
