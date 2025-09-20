using System;
using System.Threading.Tasks;
using CleanArchitecture.API.Attributes;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Users.Commands.CreateUser;
using CleanArchitecture.Application.Features.Users.Commands.UpdateUser;
using CleanArchitecture.Application.Features.Users.Commands.UpdateUserRoles;
using CleanArchitecture.Application.Features.Users.Queries.GetAllUsers;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
using CleanArchitecture.Application.Features.Users.Queries.GetUserPermissions;
using CleanArchitecture.Application.Features.Users.Queries.GetUserRoles;
using CleanArchitecture.Application.Features.Users.Queries.GetUsersPaginated;
using CleanArchitecture.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class UsersController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly ILocalizationService _localizationService;

    public UsersController(IMediator mediator, ILocalizationService localizationService)
    {
      _mediator = mediator;
      _localizationService = localizationService;
    }

    [HttpGet("paginated")]
    [Authorize]
    [RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.Admin, PermissionConstants.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<PaginationResponse<UserDto>>>> GetUsersPaginated([FromQuery] GetUsersPaginatedRequestDto request)
    {
      var query = new GetUsersPaginatedQuery { Request = request };
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<PaginationResponse<UserDto>>.SuccessResponse(result));
    }

    [HttpGet]
    [Authorize(Policy = PermissionConstants.ManageUsers)]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
    {
      var query = new GetAllUsersQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result));
    }

    [HttpGet("id/{id}")]
    [Authorize(Policy = PermissionConstants.ManageUsers)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(Guid id)
    {
      var query = new GetUserByIdQuery { Id = id };
      var result = await _mediator.Send(query);

      if (result == null)
        return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

      return Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.ManageUsers)]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto userDto)
    {
      var command = new CreateUserCommand { User = userDto };
      var result = await _mediator.Send(command);
      return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, ApiResponse<UserDto>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_CREATED")));
    }

    [HttpPut("id/{id}")]
    [Authorize(Policy = PermissionConstants.ManageUsers)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserDto userDto)
    {
      try
      {
        userDto.Id = id; // Ensure the ID from the route is used
        var command = new UpdateUserCommand { User = userDto };
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<UserDto>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_UPDATED")));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
      }
    }

    /// <summary>
    /// Obtiene todos los roles de un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Lista de roles del usuario</returns>
    [HttpGet("id/{id}/roles")]
    [Authorize(Policy = PermissionConstants.ManageUserRoles)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserRoles(Guid id)
    {
      try
      {
        var query = new GetUserRolesQuery(id);
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
    /// Actualiza los roles de un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="request">Lista de IDs de roles a asignar</param>
    /// <returns>Lista actualizada de roles del usuario</returns>
    [HttpPut("id/{id}/roles")]
    [Authorize(Policy = PermissionConstants.ManageUserRoles)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UpdateUserRolesRequestDto request)
    {
      try
      {
        var command = new UpdateUserRolesCommand(id, request.RoleIds);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_ROLES_UPDATED")));
      }
      catch (UserNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<RoleDto>>.ErrorResponse(ex.Message));
      }
    }

    /// <summary>
    /// Obtiene todos los permisos de un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Lista de permisos del usuario a través de sus roles</returns>
    [HttpGet("id/{id}/permissions")]
    [Authorize(Policy = PermissionConstants.ManageUserRoles)]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserPermissions(Guid id)
    {
      try
      {
        var query = new GetUserPermissionsQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_PERMISSIONS_RETRIEVED")));
      }
      catch (UserNotFoundByIdError ex)
      {
        return NotFound(ApiResponse<List<PermissionDto>>.ErrorResponse(ex.Message));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<PermissionDto>>.ErrorResponse(ex.Message));
      }
    }

    // ===== EJEMPLOS DE USO DE LOS NUEVOS ATRIBUTOS DE AUTORIZACIÓN =====

    /// <summary>
    /// Ejemplo: Usuario necesita CUALQUIERA de estos permisos (manage.users O admin)
    /// </summary>
    [HttpGet("example/any-permission")]
    [RequireAnyPermission(PermissionConstants.ManageUsers, PermissionConstants.Admin)]
    public ActionResult<ApiResponse<string>> ExampleAnyPermission()
    {
      return Ok(ApiResponse<string>.SuccessResponse("Usuario tiene al menos uno de los permisos requeridos"));
    }

    /// <summary>
    /// Ejemplo: Usuario necesita TODOS estos permisos (manage.users Y manage.roles)
    /// </summary>
    [HttpGet("example/all-permissions")]
    [RequireAllPermissions(PermissionConstants.ManageUsers, PermissionConstants.ManageRoles)]
    public ActionResult<ApiResponse<string>> ExampleAllPermissions()
    {
      return Ok(ApiResponse<string>.SuccessResponse("Usuario tiene todos los permisos requeridos"));
    }
  }
}
