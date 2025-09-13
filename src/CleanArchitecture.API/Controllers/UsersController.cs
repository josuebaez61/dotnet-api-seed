using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Features.Users.Commands.CreateUser;
using CleanArchitecture.Application.Features.Users.Queries.GetAllUsers;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
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

    public UsersController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet("paginated")]
    [Authorize(Policy = PermissionConstants.Users.Read)]
    public async Task<ActionResult<ApiResponse<PaginationResponse<UserDto>>>> GetUsersPaginated([FromQuery] GetUsersPaginatedRequestDto request)
    {
      var query = new GetUsersPaginatedQuery { Request = request };
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<PaginationResponse<UserDto>>.SuccessResponse(result));
    }

    [HttpGet]
    [Authorize(Policy = PermissionConstants.Users.Read)]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
    {
      var query = new GetAllUsersQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    [Authorize(Policy = PermissionConstants.Users.Read)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(Guid id)
    {
      var query = new GetUserByIdQuery { Id = id };
      var result = await _mediator.Send(query);

      if (result == null)
        return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

      return Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.Users.Write)]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto userDto)
    {
      var command = new CreateUserCommand { User = userDto };
      var result = await _mediator.Send(command);
      return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, ApiResponse<UserDto>.SuccessResponse(result));
    }
  }
}
