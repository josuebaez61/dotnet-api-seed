using System.Threading.Tasks;
using CleanArchitecture.API.Attributes;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Users.Commands.ActivateUser;
using CleanArchitecture.Application.Features.Users.Commands.AssignRoleToUser;
using CleanArchitecture.Application.Features.Users.Commands.CreateUser;
using CleanArchitecture.Application.Features.Users.Commands.DeactivateUser;
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
    [RequireAnyPermission(PermissionNames.ManageUsers, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<PaginationResponse<UserDto>>>> GetUsersPaginated([FromQuery] GetUsersPaginatedRequestDto request)
    {
      var query = new GetUsersPaginatedQuery { Request = request };
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<PaginationResponse<UserDto>>.SuccessResponse(result));
    }

    [HttpGet]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUsers, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
    {
      var query = new GetAllUsersQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result));
    }

    [HttpGet("id/{id}")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUsers, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(Guid id)
    {
      var query = new GetUserByIdQuery { Id = id };
      var result = await _mediator.Send(query);

      if (result == null)
        return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

      return Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpPost]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUsers, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto userDto)
    {
      var command = new CreateUserCommand { User = userDto };
      var result = await _mediator.Send(command);
      return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, ApiResponse<UserDto>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_CREATED")));
    }

    [HttpPut("id/{id}")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUsers, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserDto userDto)
    {
      userDto.Id = id; // Ensure the ID from the route is used
      var command = new UpdateUserCommand { User = userDto };
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<UserDto>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_UPDATED")));
    }

    /// <summary>
    /// Gets all roles of a user by their ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of user roles</returns>
    [HttpGet("id/{id}/roles")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUserRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserRoles(Guid id)
    {
      var query = new GetUserRolesQuery(id);
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Updates the roles of a user by their ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">List of role IDs to assign</param>
    /// <returns>Updated list of user roles</returns>
    [HttpPut("id/{id}/roles")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUserRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UpdateUserRolesRequestDto request)
    {
      var command = new UpdateUserRolesCommand(id, request.RoleIds);
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_ROLES_UPDATED")));
    }

    /// <summary>
    /// Assigns a single role to a user by their ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Role ID to assign</param>
    /// <returns>Assigned role information</returns>
    [HttpPatch("id/{id}/roles")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUserRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRoleToUser(Guid id, [FromBody] AssignRoleToUserRequestDto request)
    {
      var command = new AssignRoleToUserCommand(id, request.RoleId);
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<RoleDto>.SuccessResponse(result, _localizationService.GetSuccessMessage("ROLE_ASSIGNED_TO_USER")));
    }

    /// <summary>
    /// Gets all permissions of a user by their ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of user permissions through their roles</returns>
    [HttpGet("id/{id}/permissions")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUserRoles, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserPermissions(Guid id)
    {
      var query = new GetUserPermissionsQuery(id);
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(result, _localizationService.GetSuccessMessage("USER_PERMISSIONS_RETRIEVED")));
    }

    /// <summary>
    /// Activates a user by their ID
    /// </summary>
    /// <param name="id">ID of the user to activate</param>
    /// <returns>Operation result</returns>
    [HttpPatch("id/{id}/activate")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUsers, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<bool>>> ActivateUser(Guid id)
    {
      var command = new ActivateUserCommand(id);
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<bool>.SuccessResponse(result, "User activated successfully"));
    }
    /// <summary>
    /// Deactivates a user by their ID
    /// </summary>
    /// <param name="id">ID of the user to deactivate</param>
    /// <returns>Operation result</returns>
    [HttpPatch("id/{id}/deactivate")]
    [Authorize]
    [RequireAnyPermission(PermissionNames.ManageUsers, PermissionNames.Admin, PermissionNames.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<bool>>> DeactivateUser(Guid id)
    {
      var command = new DeactivateUserCommand(id);
      var result = await _mediator.Send(command);
      return Ok(ApiResponse<bool>.SuccessResponse(result, "User deactivated successfully"));
    }
  }
}
