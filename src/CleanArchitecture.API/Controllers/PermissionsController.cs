using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Permissions.Commands.CreatePermission;
using CleanArchitecture.Application.Features.Permissions.Queries.GetAllPermissions;
using CleanArchitecture.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class PermissionsController : ControllerBase
  {
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = PermissionConstants.Permissions.Read)]
    public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetAllPermissions()
    {
      try
      {
        var query = new GetAllPermissionsQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<PermissionDto>>.ErrorResponse(ex.Message));
      }
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.Permissions.Write)]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> CreatePermission([FromBody] CreatePermissionDto request)
    {
      try
      {
        var command = new CreatePermissionCommand { Permission = request };
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<PermissionDto>.SuccessResponse(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<PermissionDto>.ErrorResponse(ex.Message));
      }
    }
  }
}
