using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserById
{
  public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
  {
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly RoleManager<Role> _roleManager;
    public GetUserByIdQueryHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
    {
      _userManager = userManager;
      _mapper = mapper;

      _roleManager = roleManager;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByIdAsync(request.Id.ToString());

      if (user == null)
        return null;

      var userDto = _mapper.Map<UserDto>(user);

      var roles = _roleManager.Roles.Where(r => r.UserRoles.Any(ur => ur.UserId == user.Id)).ToList();

      userDto.Roles = _mapper.Map<List<RoleDto>>(roles);

      return userDto;
    }
  }
}
