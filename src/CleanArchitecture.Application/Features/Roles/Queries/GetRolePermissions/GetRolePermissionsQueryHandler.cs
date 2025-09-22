using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRolePermissions
{
  public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, List<PermissionDto>>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IPermissionService _permissionService;

    private readonly IMapper _mapper;

    public GetRolePermissionsQueryHandler(
        RoleManager<Role> roleManager,
        IPermissionService permissionService,
        IMapper mapper)
    {
      _roleManager = roleManager;
      _permissionService = permissionService;
      _mapper = mapper;
    }

    public async Task<List<PermissionDto>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
      // Get role information
      var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Get role's assigned permissions (including hierarchical)
      var rolePermissions = await _permissionService.GetRolePermissionsAsync(request.RoleId);
      var response = new List<PermissionDto>();
      foreach (var permission in rolePermissions)
      {
        response.Add(_mapper.Map<PermissionDto>(permission));
      }

      return response;
    }
  }
}
