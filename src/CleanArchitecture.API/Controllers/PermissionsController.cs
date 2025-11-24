using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.API.Attributes;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Permissions.Queries.GetAllPermissions;
using CleanArchitecture.Application.Features.Roles.Queries.GetPermissionsByResource;
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
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetAllPermissions()
    {
      var query = new GetAllPermissionsQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtiene los permisos agrupados por Resource con su orden definido
    /// </summary>
    /// <returns>Lista de permisos agrupados por resource con propiedad Order</returns>
    [HttpGet("by-resource")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<PermissionsByResourceDto>>>> GetPermissionsByResource()
    {
      var query = new GetPermissionsByResourceQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<PermissionsByResourceDto>>.SuccessResponse(result));
    }
  }
}
