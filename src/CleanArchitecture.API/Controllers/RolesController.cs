using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.API.Attributes;
using CleanArchitecture.Application.Common.Exceptions;
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
      try
      {
        var query = new GetAllRolesQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
      }
    }

    [HttpGet("id/{roleId}")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById([FromRoute] Guid roleId)
    {
      try
      {
        var query = new GetRoleByIdQuery { RoleId = roleId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<RoleDto>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<RoleDto>.ErrorResponse(ex.Message));
      }
    }

    [HttpGet("id/{roleId}/permissions")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetRolePermissions([FromRoute] Guid roleId)
    {
      try
      {
        var query = new GetRolePermissionsQuery { RoleId = roleId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<PermissionDto>>.ErrorResponse(ex.Message));
      }
    }

    [HttpPost]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleDto request)
    {
      try
      {
        var command = new CreateRoleCommand { Role = request };
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<RoleDto>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<RoleDto>.ErrorResponse(ex.Message));
      }
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
      try
      {
        var query = new GetRoleUserCountQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<RoleUserCountDto>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<RoleUserCountDto>.ErrorResponse(ex.Message));
      }
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
      try
      {
        var query = new GetAssignableRolesQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result));
      }
      catch (UserNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
      }
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
      try
      {
        var query = new GetAssignableUsersQuery { RoleId = roleId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<UserOptionDto>>.SuccessResponse(result));
      }
      catch (RoleNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<List<UserOptionDto>>.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<UserOptionDto>>.ErrorResponse(ex.Message));
      }
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
      try
      {
        var command = new UpdateRoleCommand(id, request);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<RoleDto>.SuccessResponse(result, "Rol actualizado exitosamente"));
      }
      catch (RoleNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<RoleDto>.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<RoleDto>.ErrorResponse(ex.Message));
      }
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
      try
      {
        var command = new AssignUsersToRoleCommand
        {
          RoleId = roleId,
          UserIds = request.UserIds
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result, "Users assigned to role successfully"));
      }
      catch (RoleNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<List<UserDto>>.ErrorResponse(ex.Message));
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ApiResponse<List<UserDto>>.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<UserDto>>.ErrorResponse(ex.Message));
      }
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
      try
      {
        var command = new DeleteRoleCommand { RoleId = roleId };
        await _mediator.Send(command);
        return Ok(ApiResponse.SuccessResponse(_localizationService.GetSuccessMessage("ROLE_DELETED")));
      }
      catch (RoleNotFoundByIdError ex)
      {
        return NotFound(ApiResponse.ErrorResponse(ex.Message));
      }
      catch (InvalidOperationException ex)
      {
        return BadRequest(ApiResponse.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse.ErrorResponse(ex.Message));
      }
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
      try
      {
        var command = new UnassignUserFromRoleCommand
        {
          RoleId = roleId,
          UserId = request.UserId
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<UserDto>.SuccessResponse(result, "User unassigned from role successfully"));
      }
      catch (RoleNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message));
      }
      catch (UserNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message));
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
      }
    }
  }
}
