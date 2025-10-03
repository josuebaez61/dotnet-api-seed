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

namespace CleanArchitecture.Application.Features.Roles.Queries.GetAssignableRoles
{
  public class GetAssignableRolesQueryHandler : IRequestHandler<GetAssignableRolesQuery, List<RoleDto>>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetAssignableRolesQueryHandler(
      RoleManager<Role> roleManager,
      UserManager<User> userManager,
      IMapper mapper)
    {
      _roleManager = roleManager;
      _userManager = userManager;
      _mapper = mapper;
    }

    public async Task<List<RoleDto>> Handle(GetAssignableRolesQuery request, CancellationToken cancellationToken)
    {
      // Get the user to verify it exists
      var user = await _userManager.FindByIdAsync(request.UserId.ToString());
      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Get all roles
      var allRoles = await _roleManager.Roles.ToListAsync(cancellationToken);

      // Get roles assigned to the user
      var userRoles = await _userManager.GetRolesAsync(user);

      // Filter out roles that the user already has
      var assignableRoles = allRoles
        .Where(role => !userRoles.Contains(role.Name))
        .OrderByDescending(r => r.UpdatedAt)
        .ToList();

      return assignableRoles.Select(role => _mapper.Map<RoleDto>(role)).ToList();
    }
  }
}
