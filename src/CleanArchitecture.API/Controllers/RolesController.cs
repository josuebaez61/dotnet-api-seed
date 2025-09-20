using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Roles.Commands.CreateRole;
using CleanArchitecture.Application.Features.Roles.Commands.UpdateRolePermissions;
using CleanArchitecture.Application.Features.Roles.Queries.GetAllRoles;
using CleanArchitecture.Application.Features.Roles.Queries.GetRoleById;
using CleanArchitecture.Application.Features.Roles.Queries.GetRolePermissions;
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
    [Authorize(Policy = PermissionConstants.ManageRoles)]
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
    [Authorize(Policy = PermissionConstants.ManageRoles)]
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
    [Authorize(Policy = PermissionConstants.ManageRolePermissions)]
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
    [Authorize(Policy = PermissionConstants.ManageRoles)]
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
    [Authorize(Policy = PermissionConstants.ManageRolePermissions)]
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
  }
}
