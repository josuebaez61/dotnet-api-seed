using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetAssignableUsers
{
  public class GetAssignableUsersQueryHandler : IRequestHandler<GetAssignableUsersQuery, List<UserOptionDto>>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetAssignableUsersQueryHandler(
      RoleManager<Role> roleManager,
      UserManager<User> userManager,
      IMapper mapper)
    {
      _roleManager = roleManager;
      _userManager = userManager;
      _mapper = mapper;
    }

    public async Task<List<UserOptionDto>> Handle(GetAssignableUsersQuery request, CancellationToken cancellationToken)
    {
      // Get the role to verify it exists
      var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Get all users
      var allUsers = await _userManager.Users.ToListAsync(cancellationToken);

      // Get users that already have this role
      var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);

      // Filter out users that already have this role
      var assignableUsers = allUsers
        .Where(user => !usersInRole.Any(ur => ur.Id == user.Id))
        .OrderBy(u => u.FirstName)
        .ThenBy(u => u.LastName)
        .ToList();

      return assignableUsers.Select(user => _mapper.Map<UserOptionDto>(user)).ToList();
    }
  }
}
