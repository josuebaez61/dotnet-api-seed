using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.API.Attributes;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Roles.Commands.CreateRole;
using CleanArchitecture.Application.Features.Roles.Commands.UpdateRole;
using CleanArchitecture.Application.Features.Roles.Commands.UpdateRolePermissions;
using CleanArchitecture.Application.Features.Roles.Queries.GetAllRoles;
using CleanArchitecture.Application.Features.Roles.Queries.GetPermissionsByResource;
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

    public RolesController(IMediator mediator)
    {
      _mediator = mediator;
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
    public async Task<ActionResult<ApiResponse<RolePermissionsDto>>> GetRolePermissions([FromRoute] Guid roleId)
    {
      try
      {
        var query = new GetRolePermissionsQuery { RoleId = roleId };
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<RolePermissionsDto>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<RolePermissionsDto>.ErrorResponse(ex.Message));
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

    [HttpPatch("id/{roleId}/permissions")]
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
      var result = await _mediator.Send(command);
      return Ok(result);
    }

    /// <summary>
    /// Obtiene la cantidad de usuarios asignados por cada rol
    /// </summary>
    /// <returns>Diccionario con la estructura { [roleId]: cantidad de usuarios }</returns>
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
    /// Obtiene los permisos agrupados por Resource con su orden definido
    /// </summary>
    /// <returns>Lista de permisos agrupados por resource con propiedad Order</returns>
    [HttpGet("permissions/by-resource")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<PermissionsByResourceDto>>>> GetPermissionsByResource()
    {
      try
      {
        var query = new GetPermissionsByResourceQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<PermissionsByResourceDto>>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<PermissionsByResourceDto>>.ErrorResponse(ex.Message));
      }
    }
  }
}
