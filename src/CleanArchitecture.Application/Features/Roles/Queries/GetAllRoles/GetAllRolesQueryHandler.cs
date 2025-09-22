using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetAllRoles
{
  public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
  {
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public GetAllRolesQueryHandler(RoleManager<Role> roleManager, IMapper mapper)
    {
      _roleManager = roleManager;
      _mapper = mapper;
    }

    public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
      var roles = await _roleManager.Roles.OrderByDescending(r => r.UpdatedAt).ToListAsync();
      return roles.Select(role => _mapper.Map<RoleDto>(role)).ToList();
    }
  }
}
